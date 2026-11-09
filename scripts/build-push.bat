@echo off
setlocal enabledelayedexpansion

echo === HospitalM-mono Build and Push ===

set /p AWS_REGION="Enter AWS region (e.g., us-east-1): "
set /p AWS_ACCOUNT_ID="Enter AWS account ID: "

set REGISTRY_URL=!AWS_ACCOUNT_ID!.dkr.ecr.!AWS_REGION!.amazonaws.com
set PROJECT_NAME=HospitalM-mono
for /f "delims=" %%i in ('powershell -Command "[char[]]($env:PROJECT_NAME.ToLower()) -join '' | ForEach-Object { $_ -replace '[^a-z0-9]', '-' }"') do set IMAGE_NAME=%%i
set IMAGE_NAME=!IMAGE_NAME:~0,100!
:: Remove leading and trailing hyphens using PowerShell
for /f "delims=" %%i in ('powershell -Command "$n = '!IMAGE_NAME!'; $n -replace '^-+','' -replace '-+$',''"') do set IMAGE_NAME=%%i
set ECR_REPO=!IMAGE_NAME!

set /p IMAGE_TAG="Enter image tag (press Enter for 'latest'): "
if "!IMAGE_TAG!"=="" set IMAGE_TAG=latest

set FULL_IMAGE_NAME=!REGISTRY_URL!/!ECR_REPO!:!IMAGE_TAG!

echo Logging in to Amazon ECR...
call aws ecr get-login-password --region !AWS_REGION! | docker login --username AWS --password-stdin !REGISTRY_URL!
if !ERRORLEVEL! neq 0 (
    echo ECR login failed
    exit /b 1
)

echo Creating ECR repository if it does not exist...
call aws ecr describe-repositories --repository-names !ECR_REPO! --region !AWS_REGION! >nul 2>&1
if !ERRORLEVEL! neq 0 (
    echo Creating ECR repository...
    call aws ecr create-repository --repository-name !ECR_REPO! --region !AWS_REGION!
)

echo Building Docker image...
docker build -f Dockerfile -t "!FULL_IMAGE_NAME!" .
if !ERRORLEVEL! neq 0 (
    echo Docker build failed
    exit /b 1
)

echo Pushing Docker image...
docker push "!FULL_IMAGE_NAME!"
if !ERRORLEVEL! neq 0 (
    echo Docker push failed
    exit /b 1
)

echo === Build and push complete ===
echo Image: !FULL_IMAGE_NAME!
