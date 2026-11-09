@echo off
setlocal enabledelayedexpansion

echo === HospitalM-mono EKS Deployment ===

set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p CLUSTER_NAME="Enter EKS cluster name: "
set /p IMAGE_URI="Enter full image URI (with registry and tag): "

echo Enter values for environment variables (press Enter to skip):
set /p DB_CONNECTION_STRING="Enter value for DB_CONNECTION_STRING (or press Enter to skip): "
set /p REDIS_HOST="Enter value for REDIS_HOST (or press Enter to skip): "
set /p REDIS_ACCESS_KEY="Enter value for REDIS_ACCESS_KEY (or press Enter to skip): "

set PROJECT_NAME=hospitalm-mono

echo Configuring kubectl for EKS cluster...
call aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo Failed to configure kubectl
    exit /b 1
)

echo Checking cluster connectivity...
call kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo Cluster connectivity check failed
    exit /b 1
)

echo Creating Kubernetes Secret...
set SECRET_ARGS=
if not "!DB_CONNECTION_STRING!"=="" (
    set SECRET_ARGS=!SECRET_ARGS! --from-literal=DB_CONNECTION_STRING="!DB_CONNECTION_STRING!"
)
if not "!REDIS_HOST!"=="" (
    set SECRET_ARGS=!SECRET_ARGS! --from-literal=REDIS_HOST="!REDIS_HOST!"
)
if not "!REDIS_ACCESS_KEY!"=="" (
    set SECRET_ARGS=!SECRET_ARGS! --from-literal=REDIS_ACCESS_KEY="!REDIS_ACCESS_KEY!"
)

if not "!SECRET_ARGS!"=="" (
    call kubectl create secret generic !PROJECT_NAME!-secrets -n !PROJECT_NAME! !SECRET_ARGS! --dry-run=client -o yaml | kubectl apply -f -
    if !ERRORLEVEL! neq 0 (
        echo Failed to create Kubernetes Secret
        exit /b 1
    )
)

echo Applying namespace...
call kubectl apply -f kubernetes/namespace.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply namespace
    exit /b 1
)

echo Replacing placeholders in deployment.yaml...
call powershell -Command "(gc kubernetes/deployment.yaml) -replace '{{IMAGE_URI}}', '!IMAGE_URI!' | Out-File -encoding UTF8 kubernetes/deployment.yaml"
call powershell -Command "(gc kubernetes/deployment.yaml) -replace '{{NAMESPACE}}', '!PROJECT_NAME!' | Out-File -encoding UTF8 kubernetes/deployment.yaml"
if not "!DB_CONNECTION_STRING!"=="" (
    call powershell -Command "(gc kubernetes/deployment.yaml) -replace '{{DB_CONNECTION_STRING}}', '!DB_CONNECTION_STRING!' | Out-File -encoding UTF8 kubernetes/deployment.yaml"
)
if not "!REDIS_HOST!"=="" (
    call powershell -Command "(gc kubernetes/deployment.yaml) -replace '{{REDIS_HOST}}', '!REDIS_HOST!' | Out-File -encoding UTF8 kubernetes/deployment.yaml"
)
if not "!REDIS_ACCESS_KEY!"=="" (
    call powershell -Command "(gc kubernetes/deployment.yaml) -replace '{{REDIS_ACCESS_KEY}}', '!REDIS_ACCESS_KEY!' | Out-File -encoding UTF8 kubernetes/deployment.yaml"
)

echo Applying deployment...
call kubectl apply -f kubernetes/deployment.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply deployment
    exit /b 1
)

echo Applying service...
call kubectl apply -f kubernetes/service.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply service
    exit /b 1
)

echo Applying ingress...
call kubectl apply -f kubernetes/ingress.yaml
if !ERRORLEVEL! neq 0 (
    echo Failed to apply ingress
    exit /b 1
)

echo Waiting for deployment rollout...
call kubectl rollout status deployment/!PROJECT_NAME! -n !PROJECT_NAME!
if !ERRORLEVEL! neq 0 (
    echo Deployment rollout failed
    exit /b 1
)

echo === Deployment complete ===
echo Pods, Services, and Ingress:
call kubectl get pods,svc,ingress -n !PROJECT_NAME!
echo.
echo Application URL: http://!PROJECT_NAME!.example.com
echo.
echo Rollback instructions:
echo   kubectl rollout undo deployment/!PROJECT_NAME! -n !PROJECT_NAME!
