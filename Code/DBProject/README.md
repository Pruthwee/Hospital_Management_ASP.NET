# Hospital Management System - Cloud-Ready Deployment

## Overview

This Hospital Management System has been modernized for cloud deployment on Google Cloud Platform (GCP) using Google Kubernetes Engine (GKE). The application is built on ASP.NET Web Forms with .NET Framework 4.8 and uses SQL Server as the database.

## Cloud Readiness Improvements

### ✅ Completed Fixes

1. **Framework Upgrade**: Upgraded from .NET Framework 4.5.2 to 4.8
2. **Connection Pooling**: Implemented proper connection pooling with Cloud SQL support
3. **Environment Variables**: Added support for environment-based configuration
4. **Session Management**: Created abstraction layer for cloud-ready session state
5. **Structured Logging**: Implemented console-based logging for container environments
6. **Resource Management**: All database connections use proper disposal patterns

### 📋 Architecture Notes

- **Current**: ASP.NET Web Forms on .NET Framework 4.8
- **Deployment**: Windows containers on GKE
- **Database**: Cloud SQL for SQL Server with Auth Proxy
- **Session State**: Configurable (Session Affinity or Memorystore Redis)

## Prerequisites

- Google Cloud Platform account
- `gcloud` CLI installed and configured
- `kubectl` installed
- Docker Desktop (for local testing)
- GCP project with billing enabled

## Quick Start

### 1. Set Up GCP Project

```bash
# Set your project ID
export PROJECT_ID="your-project-id"
export REGION="us-central1"
export ZONE="us-central1-a"

# Set the project
gcloud config set project $PROJECT_ID
```

### 2. Create GKE Cluster with Windows Node Pool

```bash
# Create cluster
gcloud container clusters create hospital-cluster \
  --enable-ip-alias \
  --num-nodes=1 \
  --zone=$ZONE

# Add Windows node pool
gcloud container node-pools create windows-pool \
  --cluster=hospital-cluster \
  --image-type=WINDOWS_LTSC_CONTAINERD \
  --machine-type=n1-standard-4 \
  --num-nodes=2 \
  --zone=$ZONE
```

### 3. Create Cloud SQL Instance

```bash
# Create SQL Server instance
gcloud sql instances create hospital-db \
  --database-version=SQLSERVER_2019_STANDARD \
  --tier=db-custom-2-7680 \
  --region=$REGION \
  --root-password=YOUR_SECURE_PASSWORD

# Create database
gcloud sql databases create DBProject --instance=hospital-db

# Create user
gcloud sql users create sqluser \
  --instance=hospital-db \
  --password=YOUR_SECURE_PASSWORD
```

### 4. Build and Push Docker Image

```bash
# Build the image
docker build -t gcr.io/$PROJECT_ID/hospital-management:latest .

# Push to Google Container Registry
docker push gcr.io/$PROJECT_ID/hospital-management:latest
```

### 5. Configure Secrets

```bash
# Create service account for Cloud SQL Proxy
gcloud iam service-accounts create cloudsql-proxy \
  --display-name="Cloud SQL Proxy"

# Grant permissions
gcloud projects add-iam-policy-binding $PROJECT_ID \
  --member="serviceAccount:cloudsql-proxy@$PROJECT_ID.iam.gserviceaccount.com" \
  --role="roles/cloudsql.client"

# Create key
gcloud iam service-accounts keys create key.json \
  --iam-account=cloudsql-proxy@$PROJECT_ID.iam.gserviceaccount.com

# Create Kubernetes secret
kubectl create secret generic cloudsql-instance-credentials \
  --from-file=service_account.json=key.json \
  --namespace=hospital-management
```

### 6. Update Deployment Configuration

Edit `kubernetes-deployment.yaml` and replace:
- `PROJECT_ID` with your GCP project ID
- `REGION` with your region
- `INSTANCE_NAME` with your Cloud SQL instance name
- `REPLACE_WITH_SECRET` with your database password

### 7. Deploy to GKE

```bash
# Apply the deployment
kubectl apply -f kubernetes-deployment.yaml

# Check deployment status
kubectl get pods -n hospital-management
kubectl get services -n hospital-management

# Get external IP
kubectl get service hospital-management -n hospital-management
```

## Configuration

### Environment Variables

The application supports the following environment variables:

| Variable | Description | Default |
|----------|-------------|---------|
| `DB_CONNECTION_STRING` | Database connection string | From Web.config |
| `CloudSqlConnectionName` | Cloud SQL instance connection name | - |
| `UseCloudSqlProxy` | Enable Cloud SQL Proxy | true |
| `MaxPoolSize` | Maximum connection pool size | 100 |
| `MinPoolSize` | Minimum connection pool size | 5 |
| `ConnectionTimeout` | Connection timeout in seconds | 30 |

### Connection String Format

```
Server=127.0.0.1,1433;Database=DBProject;User Id=sqluser;Password=***;Connection Timeout=30;Max Pool Size=100;Min Pool Size=5;Pooling=true;
```

## Session State Options

### Option 1: Session Affinity (Default)

The deployment uses `sessionAffinity: ClientIP` to route requests from the same client to the same pod. This works for small to medium deployments.

**Pros**: Simple, no additional infrastructure
**Cons**: Limits horizontal scaling, session loss on pod restart

### Option 2: Memorystore Redis (Recommended)

For production deployments, use Memorystore Redis for distributed session state.

```bash
# Create Redis instance
gcloud redis instances create hospital-sessions \
  --size=1 \
  --region=$REGION \
  --redis-version=redis_6_x

# Get Redis host
gcloud redis instances describe hospital-sessions \
  --region=$REGION \
  --format="value(host)"
```

Update `Web.config` with Redis configuration (see CLOUD_DEPLOYMENT_GUIDE.md).

## Monitoring and Logging

### View Logs

```bash
# View application logs
kubectl logs -f deployment/hospital-management -n hospital-management -c app

# View Cloud SQL Proxy logs
kubectl logs -f deployment/hospital-management -n hospital-management -c cloud-sql-proxy
```

### Cloud Logging

Logs are automatically sent to Cloud Logging. View them in the GCP Console:
- Navigation Menu > Logging > Logs Explorer
- Filter by resource: `k8s_container`

### Monitoring

Set up monitoring in Cloud Console:
- Navigation Menu > Monitoring > Dashboards
- Create dashboard for:
  - Pod CPU/Memory usage
  - Database connection pool metrics
  - HTTP request rates and latency

## Scaling

### Manual Scaling

```bash
# Scale deployment
kubectl scale deployment hospital-management \
  --replicas=5 \
  --namespace=hospital-management
```

### Auto-scaling

The deployment includes a HorizontalPodAutoscaler that automatically scales based on CPU and memory usage:
- Min replicas: 2
- Max replicas: 10
- Target CPU: 70%
- Target Memory: 80%

## Troubleshooting

### Pod Not Starting

```bash
# Check pod status
kubectl describe pod <pod-name> -n hospital-management

# Check events
kubectl get events -n hospital-management --sort-by='.lastTimestamp'
```

### Database Connection Issues

```bash
# Test Cloud SQL Proxy
kubectl exec -it <pod-name> -n hospital-management -c cloud-sql-proxy -- /bin/sh

# Check connection string
kubectl get secret db-credentials -n hospital-management -o yaml
```

### Session State Issues

If users are being logged out:
1. Verify session affinity is enabled
2. Check pod restarts: `kubectl get pods -n hospital-management`
3. Consider migrating to Redis-based session state

## Security Best Practices

1. **Secrets Management**: Use GCP Secret Manager for sensitive data
2. **Network Policies**: Implement Kubernetes network policies
3. **HTTPS**: Enable SSL/TLS with managed certificates
4. **IAM**: Use least-privilege service accounts
5. **Vulnerability Scanning**: Enable Container Analysis API

## Cost Optimization

1. **Right-size resources**: Adjust CPU/memory requests based on actual usage
2. **Use preemptible nodes**: For non-production environments
3. **Enable cluster autoscaling**: Scale nodes based on demand
4. **Use committed use discounts**: For predictable workloads

## Backup and Disaster Recovery

### Database Backups

```bash
# Enable automated backups
gcloud sql instances patch hospital-db \
  --backup-start-time=03:00 \
  --enable-bin-log

# Create on-demand backup
gcloud sql backups create \
  --instance=hospital-db
```

### Application Backups

- Container images are stored in GCR with versioning
- Kubernetes configurations should be version-controlled in Git
- Use GCP Cloud Storage for application data backups

## Support

For issues or questions:
1. Check the [CLOUD_DEPLOYMENT_GUIDE.md](CLOUD_DEPLOYMENT_GUIDE.md) for detailed documentation
2. Review GCP documentation for Cloud SQL, GKE, and Memorystore
3. Check application logs in Cloud Logging

## License

[Your License Here]

## Contributors

[Your Team/Organization]
