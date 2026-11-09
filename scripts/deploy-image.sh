#!/bin/bash
set -e
set -o pipefail

echo "=== HospitalM-mono EKS Deployment ==="

read -p "Enter AWS region (e.g., us-east-1): " AWS_REGION
read -p "Enter EKS cluster name: " CLUSTER_NAME
read -p "Enter full image URI (with registry and tag): " IMAGE_URI

# Environment variables needed by the application
echo "Enter values for environment variables (press Enter to skip):"
read -p "Enter value for DB_CONNECTION_STRING (or press Enter to skip): " DB_CONNECTION_STRING
read -p "Enter value for REDIS_HOST (or press Enter to skip): " REDIS_HOST
read -s -p "Enter value for REDIS_ACCESS_KEY (or press Enter to skip): " REDIS_ACCESS_KEY
echo

PROJECT_NAME="hospitalm-mono"

echo "Configuring kubectl for EKS cluster..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo "Checking cluster connectivity..."
kubectl cluster-info || exit 1

# Build the secret arguments
SECRET_ARGS=""
if [ -n "$DB_CONNECTION_STRING" ]; then
    SECRET_ARGS="$SECRET_ARGS --from-literal=DB_CONNECTION_STRING=\"$DB_CONNECTION_STRING\""
fi
if [ -n "$REDIS_HOST" ]; then
    SECRET_ARGS="$SECRET_ARGS --from-literal=REDIS_HOST=\"$REDIS_HOST\""
fi
if [ -n "$REDIS_ACCESS_KEY" ]; then
    SECRET_ARGS="$SECRET_ARGS --from-literal=REDIS_ACCESS_KEY=\"$REDIS_ACCESS_KEY\""
fi

echo "Creating Kubernetes Secret..."
if [ -n "$SECRET_ARGS" ]; then
    eval kubectl create secret generic "${PROJECT_NAME}-secrets" -n "$PROJECT_NAME" $SECRET_ARGS --dry-run=client -o yaml | kubectl apply -f -
fi

echo "Applying namespace..."
kubectl apply -f kubernetes/namespace.yaml

echo "Replacing placeholders in deployment.yaml..."
sed -i.bak "s|{{IMAGE_URI}}|$IMAGE_URI|g" kubernetes/deployment.yaml
sed -i.bak "s|{{NAMESPACE}}|$PROJECT_NAME|g" kubernetes/deployment.yaml
if [ -n "$DB_CONNECTION_STRING" ]; then
    sed -i.bak "s|{{DB_CONNECTION_STRING}}|$DB_CONNECTION_STRING|g" kubernetes/deployment.yaml
fi
if [ -n "$REDIS_HOST" ]; then
    sed -i.bak "s|{{REDIS_HOST}}|$REDIS_HOST|g" kubernetes/deployment.yaml
fi
if [ -n "$REDIS_ACCESS_KEY" ]; then
    sed -i.bak "s|{{REDIS_ACCESS_KEY}}|$REDIS_ACCESS_KEY|g" kubernetes/deployment.yaml
fi
rm -f kubernetes/deployment.yaml.bak

echo "Applying deployment..."
kubectl apply -f kubernetes/deployment.yaml

echo "Applying service..."
kubectl apply -f kubernetes/service.yaml

echo "Applying ingress..."
kubectl apply -f kubernetes/ingress.yaml

echo "Waiting for deployment rollout..."
kubectl rollout status deployment/"$PROJECT_NAME" -n "$PROJECT_NAME"

echo "=== Deployment complete ==="
echo "Pods, Services, and Ingress:"
kubectl get pods,svc,ingress -n "$PROJECT_NAME"
echo
echo "Application URL: http://$(kubectl get ingress "$PROJECT_NAME-ingress" -n "$PROJECT_NAME" -o jsonpath='{.status.loadBalancer.ingress[0].hostname}')"
echo
echo "Rollback instructions:"
echo "  kubectl rollout undo deployment/$PROJECT_NAME -n $PROJECT_NAME"
