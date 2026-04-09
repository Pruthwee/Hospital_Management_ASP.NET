#!/bin/bash
# AWS Deployment Script for Hospital Management System
# This script sets up the required AWS infrastructure for cloud deployment

set -e

# Configuration
PROJECT_NAME="hospital-management"
REGION="us-east-1"
DB_INSTANCE_CLASS="db.t3.medium"
CACHE_NODE_TYPE="cache.t3.medium"
VPC_CIDR="10.0.0.0/16"

echo "=========================================="
echo "AWS Infrastructure Setup"
echo "Project: $PROJECT_NAME"
echo "Region: $REGION"
echo "=========================================="

# Function to check if AWS CLI is installed
check_aws_cli() {
    if ! command -v aws &> /dev/null; then
        echo "ERROR: AWS CLI is not installed. Please install it first."
        exit 1
    fi
    echo "✓ AWS CLI found"
}

# Function to create VPC
create_vpc() {
    echo ""
    echo "Creating VPC..."
    VPC_ID=$(aws ec2 create-vpc \
        --cidr-block $VPC_CIDR \
        --region $REGION \
        --tag-specifications "ResourceType=vpc,Tags=[{Key=Name,Value=$PROJECT_NAME-vpc}]" \
        --query 'Vpc.VpcId' \
        --output text)
    echo "✓ VPC created: $VPC_ID"
    
    # Enable DNS hostnames
    aws ec2 modify-vpc-attribute \
        --vpc-id $VPC_ID \
        --enable-dns-hostnames \
        --region $REGION
    echo "✓ DNS hostnames enabled"
}

# Function to create subnets
create_subnets() {
    echo ""
    echo "Creating subnets..."
    
    # Public subnet 1
    PUBLIC_SUBNET_1=$(aws ec2 create-subnet \
        --vpc-id $VPC_ID \
        --cidr-block 10.0.1.0/24 \
        --availability-zone ${REGION}a \
        --tag-specifications "ResourceType=subnet,Tags=[{Key=Name,Value=$PROJECT_NAME-public-1}]" \
        --query 'Subnet.SubnetId' \
        --output text)
    echo "✓ Public subnet 1 created: $PUBLIC_SUBNET_1"
    
    # Public subnet 2
    PUBLIC_SUBNET_2=$(aws ec2 create-subnet \
        --vpc-id $VPC_ID \
        --cidr-block 10.0.2.0/24 \
        --availability-zone ${REGION}b \
        --tag-specifications "ResourceType=subnet,Tags=[{Key=Name,Value=$PROJECT_NAME-public-2}]" \
        --query 'Subnet.SubnetId' \
        --output text)
    echo "✓ Public subnet 2 created: $PUBLIC_SUBNET_2"
    
    # Private subnet 1
    PRIVATE_SUBNET_1=$(aws ec2 create-subnet \
        --vpc-id $VPC_ID \
        --cidr-block 10.0.11.0/24 \
        --availability-zone ${REGION}a \
        --tag-specifications "ResourceType=subnet,Tags=[{Key=Name,Value=$PROJECT_NAME-private-1}]" \
        --query 'Subnet.SubnetId' \
        --output text)
    echo "✓ Private subnet 1 created: $PRIVATE_SUBNET_1"
    
    # Private subnet 2
    PRIVATE_SUBNET_2=$(aws ec2 create-subnet \
        --vpc-id $VPC_ID \
        --cidr-block 10.0.12.0/24 \
        --availability-zone ${REGION}b \
        --tag-specifications "ResourceType=subnet,Tags=[{Key=Name,Value=$PROJECT_NAME-private-2}]" \
        --query 'Subnet.SubnetId' \
        --output text)
    echo "✓ Private subnet 2 created: $PRIVATE_SUBNET_2"
}

# Function to create Internet Gateway
create_internet_gateway() {
    echo ""
    echo "Creating Internet Gateway..."
    IGW_ID=$(aws ec2 create-internet-gateway \
        --tag-specifications "ResourceType=internet-gateway,Tags=[{Key=Name,Value=$PROJECT_NAME-igw}]" \
        --query 'InternetGateway.InternetGatewayId' \
        --output text)
    echo "✓ Internet Gateway created: $IGW_ID"
    
    # Attach to VPC
    aws ec2 attach-internet-gateway \
        --vpc-id $VPC_ID \
        --internet-gateway-id $IGW_ID \
        --region $REGION
    echo "✓ Internet Gateway attached to VPC"
}

# Function to create RDS instance
create_rds() {
    echo ""
    echo "Creating RDS SQL Server instance..."
    echo "NOTE: This will take 10-15 minutes..."
    
    # Create DB subnet group
    aws rds create-db-subnet-group \
        --db-subnet-group-name $PROJECT_NAME-db-subnet \
        --db-subnet-group-description "Subnet group for $PROJECT_NAME" \
        --subnet-ids $PRIVATE_SUBNET_1 $PRIVATE_SUBNET_2 \
        --region $REGION
    echo "✓ DB subnet group created"
    
    # Create security group for RDS
    RDS_SG=$(aws ec2 create-security-group \
        --group-name $PROJECT_NAME-rds-sg \
        --description "Security group for RDS" \
        --vpc-id $VPC_ID \
        --query 'GroupId' \
        --output text)
    echo "✓ RDS security group created: $RDS_SG"
    
    # Allow SQL Server port from application
    aws ec2 authorize-security-group-ingress \
        --group-id $RDS_SG \
        --protocol tcp \
        --port 1433 \
        --cidr 10.0.0.0/16 \
        --region $REGION
    echo "✓ RDS security group rules configured"
    
    # Create RDS instance
    aws rds create-db-instance \
        --db-instance-identifier $PROJECT_NAME-db \
        --db-instance-class $DB_INSTANCE_CLASS \
        --engine sqlserver-ex \
        --master-username admin \
        --master-user-password "ChangeMe123!" \
        --allocated-storage 20 \
        --vpc-security-group-ids $RDS_SG \
        --db-subnet-group-name $PROJECT_NAME-db-subnet \
        --backup-retention-period 7 \
        --multi-az \
        --region $REGION
    echo "✓ RDS instance creation initiated"
    echo "  Instance ID: $PROJECT_NAME-db"
    echo "  WARNING: Default password set. Change immediately after deployment!"
}

# Function to create ElastiCache Redis
create_elasticache() {
    echo ""
    echo "Creating ElastiCache Redis cluster..."
    
    # Create cache subnet group
    aws elasticache create-cache-subnet-group \
        --cache-subnet-group-name $PROJECT_NAME-cache-subnet \
        --cache-subnet-group-description "Subnet group for $PROJECT_NAME cache" \
        --subnet-ids $PRIVATE_SUBNET_1 $PRIVATE_SUBNET_2 \
        --region $REGION
    echo "✓ Cache subnet group created"
    
    # Create security group for Redis
    REDIS_SG=$(aws ec2 create-security-group \
        --group-name $PROJECT_NAME-redis-sg \
        --description "Security group for Redis" \
        --vpc-id $VPC_ID \
        --query 'GroupId' \
        --output text)
    echo "✓ Redis security group created: $REDIS_SG"
    
    # Allow Redis port from application
    aws ec2 authorize-security-group-ingress \
        --group-id $REDIS_SG \
        --protocol tcp \
        --port 6379 \
        --cidr 10.0.0.0/16 \
        --region $REGION
    echo "✓ Redis security group rules configured"
    
    # Create Redis cluster
    aws elasticache create-cache-cluster \
        --cache-cluster-id $PROJECT_NAME-redis \
        --cache-node-type $CACHE_NODE_TYPE \
        --engine redis \
        --num-cache-nodes 1 \
        --cache-subnet-group-name $PROJECT_NAME-cache-subnet \
        --security-group-ids $REDIS_SG \
        --region $REGION
    echo "✓ Redis cluster creation initiated"
    echo "  Cluster ID: $PROJECT_NAME-redis"
}

# Function to create ECS cluster
create_ecs_cluster() {
    echo ""
    echo "Creating ECS cluster..."
    
    aws ecs create-cluster \
        --cluster-name $PROJECT_NAME-cluster \
        --region $REGION
    echo "✓ ECS cluster created: $PROJECT_NAME-cluster"
}

# Function to output configuration
output_configuration() {
    echo ""
    echo "=========================================="
    echo "Infrastructure Setup Complete!"
    echo "=========================================="
    echo ""
    echo "Next Steps:"
    echo "1. Wait for RDS instance to be available (10-15 minutes)"
    echo "2. Wait for Redis cluster to be available (5-10 minutes)"
    echo "3. Get RDS endpoint:"
    echo "   aws rds describe-db-instances --db-instance-identifier $PROJECT_NAME-db --query 'DBInstances[0].Endpoint.Address' --output text"
    echo ""
    echo "4. Get Redis endpoint:"
    echo "   aws elasticache describe-cache-clusters --cache-cluster-id $PROJECT_NAME-redis --show-cache-node-info --query 'CacheClusters[0].CacheNodes[0].Endpoint.Address' --output text"
    echo ""
    echo "5. Update environment variables in your deployment:"
    echo "   DB_CONNECTION_STRING=Server=<RDS_ENDPOINT>;Database=DBProject;User Id=admin;Password=ChangeMe123!;"
    echo "   REDIS_HOST=<REDIS_ENDPOINT>"
    echo "   REDIS_PORT=6379"
    echo ""
    echo "6. Build and push Docker image to ECR"
    echo "7. Deploy to ECS"
    echo ""
    echo "Resource IDs:"
    echo "  VPC: $VPC_ID"
    echo "  Public Subnets: $PUBLIC_SUBNET_1, $PUBLIC_SUBNET_2"
    echo "  Private Subnets: $PRIVATE_SUBNET_1, $PRIVATE_SUBNET_2"
    echo "  RDS Security Group: $RDS_SG"
    echo "  Redis Security Group: $REDIS_SG"
    echo ""
    echo "IMPORTANT: Change the default RDS password immediately!"
    echo "=========================================="
}

# Main execution
main() {
    check_aws_cli
    create_vpc
    create_subnets
    create_internet_gateway
    create_rds
    create_elasticache
    create_ecs_cluster
    output_configuration
}

# Run main function
main
