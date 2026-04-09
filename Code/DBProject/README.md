# Hospital Management System - Cloud Deployment

## Overview

This Hospital Management System has been modernized for cloud deployment on AWS. The application uses ASP.NET Web Forms with .NET Framework 4.8 and has been configured for cloud-native patterns including:

- ✅ Environment-based configuration
- ✅ Distributed session state (Redis)
- ✅ Connection pooling for database
- ✅ Cloud-ready logging and monitoring
- ✅ Container support (Windows containers)

## Quick Start

### Prerequisites

- Docker Desktop with Windows containers enabled
- .NET Framework 4.8 SDK
- Visual Studio 2019 or later (for development)
- AWS CLI (for cloud deployment)

### Local Development

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DBProject
   ```

2. **Set environment variables**
   ```bash
   # Windows
   set DB_CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=DBProject;Integrated Security=True;
   set REDIS_HOST=localhost
   set REDIS_PORT=6379
   ```

3. **Run with Docker Compose**
   ```bash
   docker-compose up
   ```

4. **Access the application**
   - Open browser: http://localhost:8080

### AWS Deployment

#### Step 1: Set up AWS Infrastructure

```bash
# Make script executable
chmod +x deploy-aws-infrastructure.sh

# Run infrastructure setup
./deploy-aws-infrastructure.sh
```

This script creates:
- VPC with public and private subnets
- RDS SQL Server instance
- ElastiCache Redis cluster
- ECS cluster
- Security groups

#### Step 2: Build and Push Docker Image

```bash
# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin <account-id>.dkr.ecr.us-east-1.amazonaws.com

# Create ECR repository
aws ecr create-repository --repository-name hospital-management --region us-east-1

# Build image
docker build -t hospital-management:latest .

# Tag image
docker tag hospital-management:latest <account-id>.dkr.ecr.us-east-1.amazonaws.com/hospital-management:latest

# Push image
docker push <account-id>.dkr.ecr.us-east-1.amazonaws.com/hospital-management:latest
```

#### Step 3: Deploy to ECS

```bash
# Create task definition
aws ecs register-task-definition --cli-input-json file://ecs-task-definition.json

# Create service
aws ecs create-service \
  --cluster hospital-management-cluster \
  --service-name hospital-management-service \
  --task-definition hospital-management:1 \
  --desired-count 2 \
  --launch-type FARGATE \
  --network-configuration "awsvpcConfiguration={subnets=[subnet-xxx,subnet-yyy],securityGroups=[sg-xxx],assignPublicIp=ENABLED}"
```

#### Step 4: Configure Environment Variables

Set the following environment variables in ECS task definition:

```json
{
  "environment": [
    {
      "name": "DB_CONNECTION_STRING",
      "value": "Server=<rds-endpoint>;Database=DBProject;User Id=admin;Password=<password>;"
    },
    {
      "name": "REDIS_HOST",
      "value": "<redis-endpoint>"
    },
    {
      "name": "REDIS_PORT",
      "value": "6379"
    }
  ]
}
```

**IMPORTANT**: Use AWS Secrets Manager for sensitive values in production.

## Configuration

### Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `DB_CONNECTION_STRING` | SQL Server connection string | `Server=mydb.rds.amazonaws.com;Database=DBProject;User Id=admin;Password=***;` |
| `REDIS_HOST` | Redis server hostname | `mycache.cache.amazonaws.com` |
| `REDIS_PORT` | Redis server port | `6379` |
| `REDIS_ACCESS_KEY` | Redis authentication key | `your-redis-key` |
| `APP_INSIGHTS_KEY` | Application Insights key | `your-instrumentation-key` |

### Database Setup

1. **Create database schema**
   ```sql
   -- Run your database migration scripts
   -- Located in /Database/Scripts/
   ```

2. **Configure connection pooling**
   - Min Pool Size: 5
   - Max Pool Size: 100
   - Connection Timeout: 30 seconds

### Session State

The application uses Redis for distributed session state:

- **Provider**: Microsoft.Web.RedisSessionStateProvider
- **Configuration**: Web.config
- **Timeout**: 20 minutes (default)

## Architecture

### Components

```
┌─────────────────────────────────────────────────────────────┐
│                     Application Load Balancer               │
│                    (Sticky Sessions Enabled)                │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                         ECS Cluster                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Task 1     │  │   Task 2     │  │   Task N     │      │
│  │ (Container)  │  │ (Container)  │  │ (Container)  │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
           │                                    │
           ▼                                    ▼
┌──────────────────────┐           ┌──────────────────────┐
│   RDS SQL Server     │           │  ElastiCache Redis   │
│   (Multi-AZ)         │           │  (Session State)     │
└──────────────────────┘           └──────────────────────┘
```

### Cloud Services Used

- **Amazon ECS**: Container orchestration
- **Amazon RDS**: SQL Server database
- **Amazon ElastiCache**: Redis for session state
- **Application Load Balancer**: Traffic distribution
- **Amazon ECR**: Container registry
- **AWS Secrets Manager**: Secrets management
- **CloudWatch**: Logging and monitoring

## Monitoring

### CloudWatch Metrics

Monitor the following metrics:

- **Application**:
  - Request count
  - Response time
  - Error rate
  - Session count

- **RDS**:
  - CPU utilization
  - Database connections
  - Read/Write IOPS
  - Storage space

- **ElastiCache**:
  - CPU utilization
  - Memory usage
  - Cache hit rate
  - Evictions

### Logging

Logs are sent to CloudWatch Logs:

- Application logs: `/aws/ecs/hospital-management/app`
- IIS logs: `/aws/ecs/hospital-management/iis`

## Scaling

### Auto Scaling Configuration

```bash
# Configure ECS service auto scaling
aws application-autoscaling register-scalable-target \
  --service-namespace ecs \
  --resource-id service/hospital-management-cluster/hospital-management-service \
  --scalable-dimension ecs:service:DesiredCount \
  --min-capacity 2 \
  --max-capacity 10

# Create scaling policy
aws application-autoscaling put-scaling-policy \
  --service-namespace ecs \
  --resource-id service/hospital-management-cluster/hospital-management-service \
  --scalable-dimension ecs:service:DesiredCount \
  --policy-name cpu-scaling-policy \
  --policy-type TargetTrackingScaling \
  --target-tracking-scaling-policy-configuration file://scaling-policy.json
```

### Scaling Limits

- **Minimum instances**: 2 (for high availability)
- **Maximum instances**: 10 (adjust based on load)
- **Target CPU utilization**: 70%
- **Scale-out cooldown**: 60 seconds
- **Scale-in cooldown**: 300 seconds

## Security

### Best Practices

1. **Use AWS Secrets Manager** for sensitive data
2. **Enable encryption at rest** for RDS and ElastiCache
3. **Use IAM roles** instead of access keys
4. **Enable VPC Flow Logs** for network monitoring
5. **Implement AWS WAF** for web application firewall
6. **Enable CloudTrail** for audit logging

### Security Groups

- **Application**: Allow inbound 80/443 from ALB
- **RDS**: Allow inbound 1433 from application security group
- **Redis**: Allow inbound 6379 from application security group

## Troubleshooting

### Common Issues

1. **Database connection fails**
   - Check security group rules
   - Verify connection string
   - Ensure RDS is in same VPC

2. **Session state not persisting**
   - Verify Redis connectivity
   - Check Redis security group
   - Validate Redis configuration in Web.config

3. **High memory usage**
   - Review ViewState usage
   - Implement output caching
   - Optimize database queries

### Debug Mode

To enable debug mode:

```xml
<!-- Web.config -->
<compilation debug="true" targetFramework="4.8"/>
```

**WARNING**: Never enable debug mode in production!

## Performance Optimization

### Recommendations

1. **Enable output caching** for static pages
2. **Implement CDN** (CloudFront) for static assets
3. **Use RDS Proxy** for connection pooling
4. **Enable compression** in IIS
5. **Optimize database indexes**
6. **Implement lazy loading** for images

### Caching Strategy

```xml
<!-- Add to Web.config -->
<system.web>
  <caching>
    <outputCacheSettings>
      <outputCacheProfiles>
        <add name="StaticContent" duration="3600" varyByParam="none"/>
      </outputCacheProfiles>
    </outputCacheSettings>
  </caching>
</system.web>
```

## Cost Optimization

### Estimated Monthly Costs

- **RDS (db.t3.medium)**: $150-200
- **ElastiCache (cache.t3.medium)**: $50-75
- **ECS (2 tasks)**: $100-150
- **ALB**: $20-30
- **Data transfer**: $10-50
- **Total**: ~$330-505/month

### Cost Reduction Tips

1. Use Reserved Instances for RDS (save 30-40%)
2. Right-size instances based on metrics
3. Implement auto-scaling to reduce idle capacity
4. Use Spot Instances for non-production
5. Enable S3 lifecycle policies for logs

## Backup and Disaster Recovery

### RDS Backups

- **Automated backups**: Enabled (7-day retention)
- **Backup window**: 03:00-04:00 UTC
- **Snapshot frequency**: Daily

### Disaster Recovery Plan

1. **RTO (Recovery Time Objective)**: 1 hour
2. **RPO (Recovery Point Objective)**: 5 minutes
3. **Multi-AZ deployment**: Enabled
4. **Cross-region replication**: Optional

## Support

### Documentation

- [Cloud Readiness Guide](CLOUD_READINESS_GUIDE.md)
- [AWS Documentation](https://docs.aws.amazon.com/)
- [ASP.NET Documentation](https://docs.microsoft.com/aspnet/)

### Contact

For issues or questions:
- Create an issue in the repository
- Contact the DevOps team
- Review CloudWatch logs

## License

[Your License Here]

## Changelog

### Version 2.0.0 (Cloud-Ready)
- ✅ Upgraded to .NET Framework 4.8
- ✅ Implemented environment-based configuration
- ✅ Added Redis session state support
- ✅ Implemented connection pooling
- ✅ Added Docker support
- ✅ Created AWS deployment scripts
- ✅ Added monitoring and logging

### Version 1.0.0 (Original)
- Initial release with Web Forms
