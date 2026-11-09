# HospitalM-mono Deployment Guide

## Prerequisites and System Requirements

### Target Platform
This application is deployed on **AWS EKS (Elastic Kubernetes Service)** using **Windows containers**.

### Prerequisites
- **AWS CLI** (v2 recommended)
- **kubectl** (matching your EKS cluster version)
- **eksctl** (for EKS cluster creation)
- **Docker Desktop** (with Windows container support enabled)
- **PowerShell 5.1+** (for Windows container builds)

### IAM Permissions Required
The deployment user/role needs:
- `AmazonEC2ContainerRegistryFullAccess`
- `AmazonEKSClusterPolicy`
- `AmazonEKSWorkerNodePolicy`
- `AmazonEKS_CNI_Policy`
- `AmazonE2C2ContainerRegistryPowerUser`

### Windows Node Pool Requirements
Windows containers on EKS require a **Windows managed node group**. The cluster must have:
- EKS version 1.14 or later
- A Windows AMI (e.g., `WINDOWS_SERVICEFABRIC_CORE_1.0` or `WINDOWS_CORE_2019_x86_64`)
- The `kubernetes.io/os: windows` node selector in the deployment

### System Requirements
- **.NET Framework 4.5.2** runtime (provided by the container base image)
- **SQL Server** database (reached via `DB_CONNECTION_STRING` environment variable)
- **Redis** cache for distributed session state (reached via `REDIS_HOST` and `REDIS_ACCESS_KEY` environment variables)

## Local Setup with Docker Compose

### Build and Run Locally
```bash
# Set environment variables
export DB_CONNECTION_STRING="Server=your-server;Database=DBProject;User Id=sa;Password=YourPassword;"
export REDIS_HOST="your-redis-host.cache.amazonaws.com"
export REDIS_ACCESS_KEY="your-redis-access-key"

# Build and run
docker-compose up --build
```

The application will be available at `http://localhost`.

## Building and Pushing the Container Image

### Using build-push.sh (Linux/macOS)
```bash
./scripts/build-push.sh
```
You will be prompted for:
1. AWS region (e.g., `us-east-1`)
2. AWS account ID
3. Image tag (press Enter for `latest`)

### Using build-push.bat (Windows)
```cmd
scripts\build-push.bat
```
Same prompts as above.

### What the Script Does
1. Logs in to Amazon ECR using `aws ecr get-login-password`
2. Creates the ECR repository if it does not exist
3. Builds the Docker image using the multi-stage Dockerfile
4. Pushes the image to ECR

## Kubernetes Deployment Walkthrough

### 1. Namespace
`kubernetes/namespace.yaml` creates a dedicated namespace `hospitalm-mono` for the application.

### 2. Deployment
`kubernetes/deployment.yaml` defines:
- **2 replicas** for high availability
- **Node selector** `kubernetes.io/os: windows` (required for Windows containers)
- **Container port 80** (IIS default)
- **Environment variables** injected from Kubernetes Secrets:
  - `DB_CONNECTION_STRING` - SQL Server connection string
  - `REDIS_HOST` - Redis cache hostname
  - `REDIS_ACCESS_KEY` - Redis access key
  - `ASPNET_ENVIRONMENT=Production`
- **Resource limits**: 250m CPU / 512Mi memory (requests), 500m CPU / 1Gi memory (limits)
- **Liveness probe**: HTTP GET `/health` on port 80, 30s initial delay, 10s interval
- **Readiness probe**: HTTP GET `/health` on port 80, 10s initial delay, 5s interval

### 3. Service
`kubernetes/service.yaml` creates a ClusterIP service mapping port 80 to the container port 80.

### 4. Ingress
`kubernetes/ingress.yaml` creates an AWS ALB ingress with:
- `kubernetes.io/ingress.class: alb`
- `alb.ingress.kubernetes.io/scheme: internet-facing`
- Host: `hospitalm-mono.example.com`

### Deploying
```bash
./scripts/deploy-image.sh
```
You will be prompted for:
1. AWS region
2. EKS cluster name
3. Full image URI (from build-push.sh output)
4. Environment variable values (DB_CONNECTION_STRING, REDIS_HOST, REDIS_ACCESS_KEY)

The script will:
1. Configure kubectl for the EKS cluster
2. Create the Kubernetes Secret with credentials
3. Apply namespace, deployment, service, and ingress manifests
4. Wait for the deployment to roll out
5. Display pod, service, and ingress status

## Configuration Management

### Environment Variables
| Variable | Description | Source |
|----------|-------------|--------|
| `DB_CONNECTION_STRING` | SQL Server connection string | Kubernetes Secret |
| `REDIS_HOST` | Redis cache hostname | Kubernetes Secret |
| `REDIS_ACCESS_KEY` | Redis access key | Kubernetes Secret |
| `ASPNET_ENVIRONMENT` | ASP.NET environment (`Production`) | Deployment env |

### Web.config Settings
- The SQL connection string in `Web.config` is left empty (configured via `DB_CONNECTION_STRING` env var)
- Session state uses the Redis session provider (configured via `REDIS_HOST` and `REDIS_ACCESS_KEY` env vars)
- `debug="false"` for production

## Security Considerations and Best Practices

1. **Non-root user**: The application runs as the default IIS user inside the Windows container.
2. **HTTPS redirection**: Configure HTTPS termination at the ALB ingress level.
3. **Security headers**: Add `X-Content-Type-Options`, `X-Frame-Options`, and `X-XSS-Protection` headers via IIS configuration or a reverse proxy.
4. **Authentication and authorization**: The application uses ASP.NET session-based authentication. Ensure the session cookie is marked as secure when served over HTTPS.
5. **CORS**: Configure CORS policies in IIS if the application serves API endpoints consumed by other origins.
6. **Data Protection**: Configure the ASP.NET machine key in Web.config for consistent authentication across multiple pod replicas.
7. **Secret management**: All credentials are stored as Kubernetes Secrets, never in the container image or source code.
8. **Image scanning**: Scan the container image for vulnerabilities before pushing to ECR.
9. **Network policies**: Restrict pod-to-pod communication using Kubernetes Network Policies.
10. **Resource limits**: Resource requests and limits are set to prevent resource exhaustion.

## .NET Notes

### Runtime Settings
- **Target Framework**: .NET Framework 4.5.2
- **Runtime Stage Base Image**: `mcr.microsoft.com/dotnet/framework/aspnet:4.5.2-windowsservercore-ltsc2019`
- **Build Stage Base Image**: `mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019`
- The SDK stage uses .NET Framework 4.8 (the latest available) to build the project, which targets 4.5.2. The runtime stage uses the 4.5.2 base image as specified by the job.

### Garbage Collector Tuning
On Windows containers, the .NET Framework GC can be tuned via environment variables:
- `COMPLUS_gcServer=1` - Enables server GC (recommended for multi-core)
- `COMPLUS_gcConcurrent=0` - Disables concurrent GC (reduces memory usage)

### Monitoring
- **Application Insights**: The application includes Application Insights 2.2.0 for telemetry. Configure the instrumentation key via the `APPINSIGHTS_INSTRUMENTATIONKEY` environment variable or in the ApplicationInsights.config file.
- **Health endpoint**: The application exposes `/health` which returns `{"status":"healthy"}` with HTTP 200.

### Kestrel Configuration
This is an IIS-hosted application (not Kestrel). IIS manages the HTTP listener on port 80 inside the container.

### Windows Container Notes
- Windows containers require a matching host OS version. The `windowsservercore-ltsc2019` base image requires Windows Server 2019 or later.
- Windows container images are significantly larger (~4-8 GB) compared to Linux containers.
- Windows containers cannot run on Linux nodes - a Windows node pool is required in EKS.
- The `nodeSelector: kubernetes.io/os: windows` is mandatory in the deployment to ensure pods are scheduled on Windows nodes.
