# Azure Deployment Configuration for Hospital Management System

## Environment Variables Required

### Database Configuration
```bash
SQL_CONNECTION_STRING="Server=tcp:<your-server>.database.windows.net,1433;Initial Catalog=DBProject;Persist Security Info=False;User ID=<username>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;ConnectRetryCount=3;ConnectRetryInterval=10;"
```

### Application Configuration
```bash
ASPNETCORE_ENVIRONMENT="Production"
APPINSIGHTS_INSTRUMENTATIONKEY="<your-app-insights-key>"
```

## Azure App Service Configuration

### Application Settings (Portal)
1. Navigate to Azure Portal > App Service > Configuration > Application Settings
2. Add the following settings:
   - `SQL_CONNECTION_STRING`: Your Azure SQL connection string
   - `ASPNETCORE_ENVIRONMENT`: Production
   - `APPINSIGHTS_INSTRUMENTATIONKEY`: Your Application Insights key

### Connection Strings (Portal)
1. Navigate to Azure Portal > App Service > Configuration > Connection Strings
2. Add connection string:
   - Name: `sqlCon1`
   - Value: Your Azure SQL connection string
   - Type: SQLAzure

## Session State Configuration

### Current Configuration (InProc)
The application currently uses InProc session state which works for single-instance deployments.

### Recommended: Azure Redis Cache for Production
For production deployments with multiple instances, configure Azure Redis Cache:

1. Create Azure Cache for Redis instance
2. Update Web.config:
```xml
<sessionState mode="Custom" customProvider="MySessionStateStore">
  <providers>
    <add name="MySessionStateStore" 
         type="Microsoft.Web.Redis.RedisSessionStateProvider" 
         host="<your-redis-cache>.redis.cache.windows.net" 
         accessKey="<your-access-key>" 
         ssl="true" />
  </providers>
</sessionState>
```

3. Install NuGet package:
```bash
Install-Package Microsoft.Web.RedisSessionStateProvider
```

## Database Migration

### Azure SQL Database Setup
1. Create Azure SQL Database
2. Configure firewall rules to allow Azure services
3. Run database schema scripts
4. Update connection string in App Service configuration

### Connection String Format
```
Server=tcp:<server-name>.database.windows.net,1433;Initial Catalog=<database-name>;Persist Security Info=False;User ID=<username>;Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;ConnectRetryCount=3;ConnectRetryInterval=10;
```

## Application Insights Configuration

### Enable Application Insights
1. Create Application Insights resource in Azure
2. Copy Instrumentation Key
3. Add to App Service Application Settings
4. Application will automatically send telemetry

## Deployment Steps

### Option 1: Visual Studio Publish
1. Right-click project > Publish
2. Select Azure App Service
3. Configure settings
4. Publish

### Option 2: Azure DevOps Pipeline
1. Create build pipeline
2. Add MSBuild task
3. Add Azure App Service Deploy task
4. Configure environment variables

### Option 3: GitHub Actions
```yaml
name: Deploy to Azure App Service

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: windows-latest
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1
    
    - name: Build
      run: msbuild /p:Configuration=Release
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: '<your-app-name>'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
```

## Performance Optimization

### Connection Pooling
- Enabled by default in ConnectionManager
- Min Pool Size: 0
- Max Pool Size: 100
- Connection Timeout: 30 seconds

### Retry Logic
- Connect Retry Count: 3
- Connect Retry Interval: 10 seconds
- Handles transient failures in Azure SQL

## Security Considerations

### SSL/TLS
- Enforce SSL connections to Azure SQL
- Use TLS 1.2 or higher

### Managed Identity (Recommended)
For enhanced security, use Azure Managed Identity:
1. Enable Managed Identity on App Service
2. Grant SQL permissions to Managed Identity
3. Update connection string to use Managed Identity:
```
Server=tcp:<server>.database.windows.net;Database=<database>;Authentication=Active Directory Managed Identity;
```

## Monitoring and Logging

### Application Insights
- Automatic request tracking
- Dependency tracking (SQL calls)
- Exception tracking
- Custom telemetry

### Log Stream
- View real-time logs in Azure Portal
- App Service > Log Stream

## Scaling Configuration

### Scale Up (Vertical)
- Increase App Service Plan tier for more resources

### Scale Out (Horizontal)
- Enable auto-scale rules
- Configure session state to use Redis Cache
- Ensure stateless application design

## Health Checks

Add health check endpoint:
```csharp
// Add to Global.asax or startup
protected void Application_Start()
{
    // Health check route
    RouteTable.Routes.MapPageRoute("health", "health", "~/Health.aspx");
}
```

## Troubleshooting

### Common Issues

1. **Connection String Not Found**
   - Verify environment variable is set
   - Check App Service Configuration

2. **Session State Issues**
   - Verify session configuration in Web.config
   - Consider migrating to Redis Cache for multi-instance

3. **Database Connection Failures**
   - Check firewall rules
   - Verify connection string format
   - Check retry logic configuration

## Migration Checklist

- [ ] Create Azure SQL Database
- [ ] Configure connection string
- [ ] Set up Application Insights
- [ ] Configure session state (Redis for production)
- [ ] Test database connectivity
- [ ] Deploy application
- [ ] Verify health checks
- [ ] Configure auto-scaling
- [ ] Set up monitoring alerts
- [ ] Test application functionality

## Support

For issues or questions:
- Azure Support: https://azure.microsoft.com/support/
- Application Insights: https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview
- Azure SQL: https://docs.microsoft.com/azure/azure-sql/
