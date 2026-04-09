# Azure Deployment Guide

## Prerequisites

- Azure Subscription
- Azure CLI installed
- Visual Studio or MSBuild
- SQL Server Management Studio (optional)

## Step 1: Build the Application

```bash
# Navigate to project directory
cd /path/to/DBProject

# Restore NuGet packages
nuget restore "Clinic Management System.csproj"

# Build the project
msbuild "Clinic Management System.csproj" /p:Configuration=Release /p:DeployOnBuild=true /p:PublishProfile=FolderProfile
```

## Step 2: Create Azure Resources

### 2.1 Create Resource Group

```bash
az group create --name hospital-mgmt-rg --location eastus
```

### 2.2 Create Azure SQL Database

```bash
# Create SQL Server
az sql server create \
  --name hospital-mgmt-sql \
  --resource-group hospital-mgmt-rg \
  --location eastus \
  --admin-user sqladmin \
  --admin-password <YourStrongPassword>

# Create Database
az sql db create \
  --resource-group hospital-mgmt-rg \
  --server hospital-mgmt-sql \
  --name DBProject \
  --service-objective S0

# Configure firewall
az sql server firewall-rule create \
  --resource-group hospital-mgmt-rg \
  --server hospital-mgmt-sql \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

### 2.3 Create Azure Cache for Redis

```bash
az redis create \
  --name hospital-mgmt-redis \
  --resource-group hospital-mgmt-rg \
  --location eastus \
  --sku Basic \
  --vm-size c0

# Get Redis connection string
az redis list-keys \
  --name hospital-mgmt-redis \
  --resource-group hospital-mgmt-rg
```

### 2.4 Create App Service

```bash
# Create App Service Plan
az appservice plan create \
  --name hospital-mgmt-plan \
  --resource-group hospital-mgmt-rg \
  --sku B1 \
  --is-linux false

# Create Web App
az webapp create \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --plan hospital-mgmt-plan \
  --runtime "ASPNET|4.8"
```

## Step 3: Configure Environment Variables

```bash
# Get SQL connection string
SQL_CONN=$(az sql db show-connection-string \
  --client ado.net \
  --server hospital-mgmt-sql \
  --name DBProject | \
  sed 's/<username>/sqladmin/g' | \
  sed 's/<password>/<YourStrongPassword>/g')

# Get Redis connection string
REDIS_KEY=$(az redis list-keys \
  --name hospital-mgmt-redis \
  --resource-group hospital-mgmt-rg \
  --query primaryKey -o tsv)

REDIS_CONN="hospital-mgmt-redis.redis.cache.windows.net:6380,password=$REDIS_KEY,ssl=True,abortConnect=False"

# Set environment variables
az webapp config appsettings set \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --settings \
    SQL_CONNECTION_STRING="$SQL_CONN" \
    REDIS_CONNECTION_STRING="$REDIS_CONN" \
    ASPNETCORE_ENVIRONMENT="Production"
```

## Step 4: Deploy Database Schema

```bash
# Connect to Azure SQL Database
sqlcmd -S hospital-mgmt-sql.database.windows.net \
  -d DBProject \
  -U sqladmin \
  -P <YourStrongPassword> \
  -i database-schema.sql
```

## Step 5: Deploy Application

### Option A: Deploy via Azure CLI

```bash
# Create deployment package
cd bin/Release
zip -r ../deploy.zip *

# Deploy to App Service
az webapp deployment source config-zip \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --src ../deploy.zip
```

### Option B: Deploy via Visual Studio

1. Right-click project → Publish
2. Select Azure → Azure App Service (Windows)
3. Select your subscription and app service
4. Click Publish

### Option C: Deploy via GitHub Actions

Create `.github/workflows/azure-deploy.yml`:

```yaml
name: Deploy to Azure

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
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
    
    - name: Restore NuGet packages
      run: nuget restore "Clinic Management System.csproj"
    
    - name: Build
      run: msbuild "Clinic Management System.csproj" /p:Configuration=Release
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: hospital-mgmt-app
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: bin/Release
```

## Step 6: Verify Deployment

```bash
# Check app status
az webapp show \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --query state

# View logs
az webapp log tail \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg

# Test health endpoint
curl https://hospital-mgmt-app.azurewebsites.net/health
```

## Step 7: Configure Monitoring

```bash
# Enable Application Insights
az monitor app-insights component create \
  --app hospital-mgmt-insights \
  --location eastus \
  --resource-group hospital-mgmt-rg \
  --application-type web

# Link to Web App
INSTRUMENTATION_KEY=$(az monitor app-insights component show \
  --app hospital-mgmt-insights \
  --resource-group hospital-mgmt-rg \
  --query instrumentationKey -o tsv)

az webapp config appsettings set \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --settings APPINSIGHTS_INSTRUMENTATIONKEY="$INSTRUMENTATION_KEY"
```

## Step 8: Configure Auto-Scaling

```bash
# Create autoscale rule
az monitor autoscale create \
  --resource-group hospital-mgmt-rg \
  --resource hospital-mgmt-plan \
  --resource-type Microsoft.Web/serverfarms \
  --name autoscale-rule \
  --min-count 1 \
  --max-count 5 \
  --count 1

# Add scale-out rule (CPU > 70%)
az monitor autoscale rule create \
  --resource-group hospital-mgmt-rg \
  --autoscale-name autoscale-rule \
  --condition "Percentage CPU > 70 avg 5m" \
  --scale out 1

# Add scale-in rule (CPU < 30%)
az monitor autoscale rule create \
  --resource-group hospital-mgmt-rg \
  --autoscale-name autoscale-rule \
  --condition "Percentage CPU < 30 avg 5m" \
  --scale in 1
```

## Step 9: Configure Custom Domain (Optional)

```bash
# Add custom domain
az webapp config hostname add \
  --webapp-name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --hostname www.yourdomain.com

# Enable HTTPS
az webapp config ssl bind \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --certificate-thumbprint <thumbprint> \
  --ssl-type SNI
```

## Step 10: Backup Configuration

```bash
# Configure automated backups
az webapp config backup create \
  --resource-group hospital-mgmt-rg \
  --webapp-name hospital-mgmt-app \
  --backup-name initial-backup \
  --container-url "<storage-container-sas-url>"

# Schedule daily backups
az webapp config backup update \
  --resource-group hospital-mgmt-rg \
  --webapp-name hospital-mgmt-app \
  --frequency 1d \
  --retain-one true \
  --retention 30
```

## Troubleshooting

### Issue: Application won't start

```bash
# Check logs
az webapp log tail --name hospital-mgmt-app --resource-group hospital-mgmt-rg

# Check configuration
az webapp config appsettings list --name hospital-mgmt-app --resource-group hospital-mgmt-rg
```

### Issue: Database connection fails

```bash
# Test connection from App Service
az webapp ssh --name hospital-mgmt-app --resource-group hospital-mgmt-rg

# Inside SSH session
sqlcmd -S hospital-mgmt-sql.database.windows.net -U sqladmin -P <password> -d DBProject
```

### Issue: Redis connection fails

```bash
# Test Redis connectivity
redis-cli -h hospital-mgmt-redis.redis.cache.windows.net -p 6380 -a <access-key> --tls
```

## Cost Optimization

### Development Environment
- App Service: B1 Basic ($13/month)
- SQL Database: S0 Standard ($15/month)
- Redis Cache: C0 Basic ($16/month)
**Total: ~$44/month**

### Production Environment
- App Service: P1V2 Premium ($73/month)
- SQL Database: S2 Standard ($75/month)
- Redis Cache: C1 Standard ($55/month)
**Total: ~$203/month**

## Security Checklist

- [ ] Enable HTTPS only
- [ ] Configure firewall rules
- [ ] Use Azure Key Vault for secrets
- [ ] Enable managed identity
- [ ] Configure network security groups
- [ ] Enable DDoS protection
- [ ] Set up Azure AD authentication
- [ ] Enable audit logging

## Maintenance Tasks

### Daily
- Monitor Application Insights
- Check error logs
- Review performance metrics

### Weekly
- Review auto-scaling events
- Check backup status
- Update dependencies

### Monthly
- Review costs
- Update security patches
- Performance optimization

## Support Resources

- Azure Support: https://azure.microsoft.com/support
- Documentation: https://docs.microsoft.com/azure
- Community: https://stackoverflow.com/questions/tagged/azure

## Rollback Procedure

```bash
# List deployment slots
az webapp deployment slot list \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg

# Swap to previous slot
az webapp deployment slot swap \
  --name hospital-mgmt-app \
  --resource-group hospital-mgmt-rg \
  --slot staging \
  --target-slot production
```

---

**Last Updated**: 2025-01-06  
**Version**: 1.0
