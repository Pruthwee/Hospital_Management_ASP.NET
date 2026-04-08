# Azure Cloud Deployment Guide
## Hospital Management System - Cloud-Ready Configuration

## Overview
This document provides comprehensive guidance for deploying the Hospital Management System to Azure Cloud. The application has been optimized for cloud deployment with connection pooling, retry logic, distributed session state support, and environment-based configuration.

## Architecture Overview

### Current State
- **Framework**: ASP.NET Web Forms 4.8
- **Database**: SQL Server with connection pooling and retry logic
- **Session Management**: In-process (with Redis support ready)
- **Monitoring**: Application Insights integration

### Cloud-Ready Features Implemented
1. ✅ Connection pooling with Azure SQL transient fault handling
2. ✅ Environment variable-based configuration
3. ✅ Health check endpoint for monitoring
4. ✅ Distributed session state support (Redis ready)
5. ✅ Application Insights integration
6. ✅ Secure configuration management

## Prerequisites

### Azure Resources Required
1. **Azure App Service** (Windows, .NET 4.8)
   - Recommended: Standard S1 or higher
   - Enable Always On
   - Enable ARR Affinity (for InProc session) or disable (for Redis session)

2. **Azure SQL Database**
   - Recommended: Standard S2 or higher
   - Enable connection pooling
   - Configure firewall rules

3. **Azure Cache for Redis** (Optional but Recommended)
   - Recommended: Standard C1 or higher
   - For distributed session state
   - Enables horizontal scaling

4. **Application Insights** (Recommended)
   - For monitoring and diagnostics
   - Automatic telemetry collection

## Deployment Steps

### Step 1: Provision Azure Resources

#### 1.1 Create Resource Group
```bash
az group create --name rg-hospital-management --location eastus
```

#### 1.2 Create Azure SQL Database
```bash
az sql server create \
  --name sql-hospital-management \
  --resource-group rg-hospital-management \
  --location eastus \
  --admin-user sqladmin \
  --admin-password <YourSecurePassword>

az sql db create \
  --resource-group rg-hospital-management \
  --server sql-hospital-management \
  --name HospitalDB \
  --service-objective S2
```

#### 1.3 Create Azure App Service
```bash
az appservice plan create \
  --name plan-hospital-management \
  --resource-group rg-hospital-management \
  --location eastus \
  --sku S1

az webapp create \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --plan plan-hospital-management \
  --runtime "ASPNET|V4.8"
```

#### 1.4 Create Azure Cache for Redis (Optional)
```bash
az redis create \
  --name redis-hospital-management \
  --resource-group rg-hospital-management \
  --location eastus \
  --sku Standard \
  --vm-size C1
```

#### 1.5 Create Application Insights
```bash
az monitor app-insights component create \
  --app appinsights-hospital-management \
  --location eastus \
  --resource-group rg-hospital-management \
  --application-type web
```

### Step 2: Configure Application Settings

#### 2.1 Configure Connection String
```bash
az webapp config connection-string set \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --connection-string-type SQLAzure \
  --settings sqlCon1="Server=tcp:sql-hospital-management.database.windows.net,1433;Initial Catalog=HospitalDB;Persist Security Info=False;User ID=sqladmin;Password=<YourPassword>;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Min Pool Size=5;Max Pool Size=100;Pooling=true;Application Name=HospitalManagement-Azure"
```

#### 2.2 Configure Application Settings
```bash
az webapp config appsettings set \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --settings \
    Environment="Production" \
    EnableDetailedErrors="false" \
    UseDistributedCache="true" \
    UseApplicationInsights="true" \
    SessionTimeout="20"
```

#### 2.3 Configure Redis Connection (if using Redis)
```bash
# Get Redis connection string
REDIS_CONNECTION=$(az redis list-keys \
  --name redis-hospital-management \
  --resource-group rg-hospital-management \
  --query primaryKey -o tsv)

az webapp config connection-string set \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --connection-string-type Custom \
  --settings RedisConnection="redis-hospital-management.redis.cache.windows.net:6380,password=$REDIS_CONNECTION,ssl=True,abortConnect=False"
```

#### 2.4 Configure Application Insights
```bash
# Get instrumentation key
APPINSIGHTS_KEY=$(az monitor app-insights component show \
  --app appinsights-hospital-management \
  --resource-group rg-hospital-management \
  --query instrumentationKey -o tsv)

az webapp config appsettings set \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --settings APPINSIGHTS_INSTRUMENTATIONKEY="$APPINSIGHTS_KEY"
```

### Step 3: Database Setup

#### 3.1 Configure Firewall Rules
```bash
# Allow Azure services
az sql server firewall-rule create \
  --resource-group rg-hospital-management \
  --server sql-hospital-management \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

# Allow your IP for management
az sql server firewall-rule create \
  --resource-group rg-hospital-management \
  --server sql-hospital-management \
  --name AllowMyIP \
  --start-ip-address <YourIP> \
  --end-ip-address <YourIP>
```

#### 3.2 Deploy Database Schema
1. Connect to Azure SQL Database using SQL Server Management Studio or Azure Data Studio
2. Run your database schema scripts
3. Create stored procedures and views
4. Populate initial data

### Step 4: Enable Distributed Session State (Optional)

If using Azure Redis Cache for session state, update Web.config:

```xml
<sessionState mode="Custom" customProvider="RedisSessionStateProvider" timeout="20">
  <providers>
    <add name="RedisSessionStateProvider" 
         type="Microsoft.Web.Redis.RedisSessionStateProvider" 
         connectionString="RedisConnection" 
         ssl="true" 
         connectionTimeoutInMilliseconds="5000" 
         operationTimeoutInMilliseconds="5000" />
  </providers>
</sessionState>
```

Install NuGet package:
```bash
Install-Package Microsoft.Web.RedisSessionStateProvider
```

### Step 5: Deploy Application

#### 5.1 Build Application
```bash
# Using Visual Studio
# 1. Right-click project -> Publish
# 2. Select Azure App Service
# 3. Choose your app service
# 4. Publish

# Using MSBuild
msbuild "Clinic Management System.csproj" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=Azure
```

#### 5.2 Deploy via Azure CLI
```bash
# Create deployment package
dotnet publish -c Release -o ./publish

# Deploy to App Service
az webapp deployment source config-zip \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --src ./publish.zip
```

### Step 6: Post-Deployment Configuration

#### 6.1 Enable Always On
```bash
az webapp config set \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --always-on true
```

#### 6.2 Configure Health Check
```bash
az webapp config set \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --health-check-path "/health"
```

#### 6.3 Enable HTTPS Only
```bash
az webapp update \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --https-only true
```

#### 6.4 Configure Custom Domain (Optional)
```bash
az webapp config hostname add \
  --webapp-name app-hospital-management \
  --resource-group rg-hospital-management \
  --hostname www.yourdomain.com
```

## Configuration Reference

### Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `SQLCONNSTR_sqlCon1` | Azure SQL connection string | Auto-configured by Azure |
| `CUSTOMCONNSTR_RedisConnection` | Redis connection string | Auto-configured by Azure |
| `APPINSIGHTS_INSTRUMENTATIONKEY` | Application Insights key | Auto-configured by Azure |
| `Environment` | Deployment environment | Production, Staging, Development |
| `EnableDetailedErrors` | Show detailed errors | false (production) |
| `UseDistributedCache` | Enable Redis session | true/false |
| `UseApplicationInsights` | Enable monitoring | true/false |
| `SessionTimeout` | Session timeout (minutes) | 20 |

### Connection String Format

#### Azure SQL Database
```
Server=tcp:{server}.database.windows.net,1433;
Initial Catalog={database};
Persist Security Info=False;
User ID={username};
Password={password};
MultipleActiveResultSets=True;
Encrypt=True;
TrustServerCertificate=False;
Connection Timeout=30;
Min Pool Size=5;
Max Pool Size=100;
Pooling=true;
Application Name=HospitalManagement-Azure
```

#### Azure Redis Cache
```
{cache-name}.redis.cache.windows.net:6380,
password={access-key},
ssl=True,
abortConnect=False
```

## Monitoring and Diagnostics

### Health Check Endpoint
- **URL**: `https://your-app.azurewebsites.net/health`
- **Response**: JSON with health status
- **Status Codes**: 
  - 200: Healthy
  - 503: Unhealthy

Example response:
```json
{
  "healthy": true,
  "database": true,
  "application": true,
  "timestamp": "2024-01-15T10:30:00Z",
  "environment": "Production"
}
```

### Application Insights Queries

#### Monitor Database Performance
```kusto
dependencies
| where type == "SQL"
| summarize avg(duration), percentile(duration, 95) by name
| order by avg_duration desc
```

#### Monitor Failed Requests
```kusto
requests
| where success == false
| summarize count() by resultCode, name
| order by count_ desc
```

#### Monitor Session State
```kusto
customEvents
| where name == "SessionStateError"
| summarize count() by tostring(customDimensions.ErrorType)
```

## Security Best Practices

### 1. Connection Strings
- ✅ Store in Azure App Service Configuration (encrypted at rest)
- ✅ Use Azure Key Vault for sensitive values
- ❌ Never commit connection strings to source control

### 2. Authentication
- ✅ Use Azure AD authentication for SQL Database (recommended)
- ✅ Enable SSL/TLS for all connections
- ✅ Implement proper session timeout

### 3. Network Security
- ✅ Configure SQL Database firewall rules
- ✅ Use Azure Private Link for database access
- ✅ Enable HTTPS only

### 4. Application Security
- ✅ Enable custom errors in production
- ✅ Implement proper input validation
- ✅ Use parameterized queries (already implemented)

## Scaling Considerations

### Horizontal Scaling
To enable horizontal scaling (multiple instances):

1. **Enable Redis Session State**
   - Required for stateless operation
   - Disable ARR Affinity in App Service

2. **Configure Auto-Scaling**
```bash
az monitor autoscale create \
  --resource-group rg-hospital-management \
  --resource app-hospital-management \
  --resource-type Microsoft.Web/sites \
  --name autoscale-hospital \
  --min-count 2 \
  --max-count 10 \
  --count 2
```

3. **Add Scale Rules**
```bash
az monitor autoscale rule create \
  --resource-group rg-hospital-management \
  --autoscale-name autoscale-hospital \
  --condition "CpuPercentage > 70 avg 5m" \
  --scale out 1
```

### Vertical Scaling
Upgrade App Service Plan tier:
```bash
az appservice plan update \
  --name plan-hospital-management \
  --resource-group rg-hospital-management \
  --sku P1V2
```

## Troubleshooting

### Common Issues

#### 1. Database Connection Failures
**Symptom**: Health check returns unhealthy, database errors

**Solutions**:
- Verify firewall rules allow App Service IP
- Check connection string is correct
- Verify SQL Database is running
- Check connection pool settings

#### 2. Session State Issues
**Symptom**: Users logged out unexpectedly

**Solutions**:
- Verify session timeout configuration
- Check Redis connection if using distributed session
- Ensure ARR Affinity is enabled (InProc) or disabled (Redis)

#### 3. Performance Issues
**Symptom**: Slow response times

**Solutions**:
- Review Application Insights performance data
- Check database query performance
- Verify connection pooling is working
- Consider scaling up/out

#### 4. Deployment Failures
**Symptom**: Deployment fails or app doesn't start

**Solutions**:
- Check deployment logs in Azure Portal
- Verify .NET Framework version (4.8)
- Check Web.config syntax
- Review Application Insights for startup errors

### Diagnostic Commands

```bash
# View application logs
az webapp log tail \
  --name app-hospital-management \
  --resource-group rg-hospital-management

# Download logs
az webapp log download \
  --name app-hospital-management \
  --resource-group rg-hospital-management \
  --log-file logs.zip

# Restart application
az webapp restart \
  --name app-hospital-management \
  --resource-group rg-hospital-management
```

## Cost Optimization

### Recommendations
1. **Use appropriate tiers**
   - Start with Standard tier
   - Scale up only when needed

2. **Enable auto-scaling**
   - Scale down during off-hours
   - Use schedule-based scaling

3. **Optimize database**
   - Use appropriate service tier
   - Enable auto-pause for dev/test

4. **Monitor costs**
   - Set up cost alerts
   - Review Azure Advisor recommendations

### Estimated Monthly Costs (USD)
- App Service (S1): ~$70
- SQL Database (S2): ~$150
- Redis Cache (C1): ~$75
- Application Insights: ~$10-50 (based on usage)
- **Total**: ~$305-375/month

## Migration Path to ASP.NET Core

### Current Limitations
The application uses ASP.NET Web Forms, which has the following cloud limitations:
- Heavy resource requirements
- Limited horizontal scaling
- ViewState overhead
- Server affinity requirements

### Recommended Migration Path
1. **Phase 1**: Current deployment (Web Forms on Azure)
   - Implement distributed session state
   - Optimize database access
   - Enable monitoring

2. **Phase 2**: Gradual migration to ASP.NET Core
   - Migrate business logic to .NET Core libraries
   - Rewrite UI using Razor Pages or Blazor
   - Implement API layer

3. **Phase 3**: Full cloud-native deployment
   - Deploy to Azure Container Apps
   - Use Azure SQL with managed identity
   - Implement microservices architecture

## Support and Maintenance

### Regular Maintenance Tasks
- [ ] Monitor Application Insights dashboards
- [ ] Review and optimize database queries
- [ ] Update security patches
- [ ] Review and rotate access keys
- [ ] Backup database regularly
- [ ] Test disaster recovery procedures

### Monitoring Checklist
- [ ] Health check endpoint responding
- [ ] Database connection pool healthy
- [ ] Session state working correctly
- [ ] No critical errors in Application Insights
- [ ] Response times within acceptable range
- [ ] Resource utilization within limits

## Additional Resources

### Documentation
- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure SQL Database Documentation](https://docs.microsoft.com/azure/sql-database/)
- [Azure Cache for Redis Documentation](https://docs.microsoft.com/azure/azure-cache-for-redis/)
- [Application Insights Documentation](https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview)

### Tools
- [Azure Portal](https://portal.azure.com)
- [Azure CLI](https://docs.microsoft.com/cli/azure/)
- [Azure PowerShell](https://docs.microsoft.com/powershell/azure/)
- [Visual Studio](https://visualstudio.microsoft.com/)

## Conclusion

This deployment guide provides a comprehensive approach to deploying the Hospital Management System to Azure Cloud. The application has been optimized for cloud deployment with:

- ✅ Connection pooling and retry logic
- ✅ Environment-based configuration
- ✅ Health monitoring
- ✅ Distributed session state support
- ✅ Application Insights integration

For production deployment, follow all security best practices and enable distributed session state for horizontal scaling capabilities.
