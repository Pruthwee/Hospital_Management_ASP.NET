#!/bin/bash
set -e

echo "=== HospitalM-mono Build and Push ==="

# Ask for the target platform's registry details
read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter AWS account ID: " AWS_ACCOUNT_ID

REGISTRY_URL="${AWS_ACCOUNT_ID}.dkr.ecr.${AWS_REGION}.amazonaws.com"
PROJECT_NAME="HospitalM-mono"
IMAGE_NAME=$(echo "$PROJECT_NAME" | tr '[:upper:]' '[:lower:]' | tr -cs 'a-z0-9' '-' | sed 's/^-*//;s/-*$//')
ECR_REPO="${IMAGE_NAME}"

read -p "Enter image tag (press Enter for 'latest'): " IMAGE_TAG
if [ -z "$IMAGE_TAG" ]; then
    IMAGE_TAG="latest"
fi

FULL_IMAGE_NAME="${REGISTRY_URL}/${ECR_REPO}:${IMAGE_TAG}"

echo "Logging in to Amazon ECR..."
aws ecr get-login-password --region "$AWS_REGION" | docker login --username AWS --password-stdin "$REGISTRY_URL"

echo "Creating ECR repository if it does not exist..."
aws ecr describe-repositories --repository-names "$ECR_REPO" --region "$AWS_REGION" >/dev/null 2>&1 || aws ecr create-repository --repository-name "$ECR_REPO" --region "$AWS_REGION"

echo "Building Docker image..."
docker build -f Dockerfile -t "$FULL_IMAGE_NAME" .

echo "Pushing Docker image..."
docker push "$FULL_IMAGE_NAME"

echo "=== Build and push complete ==="
echo "Image: $FULL_IMAGE_NAME"
