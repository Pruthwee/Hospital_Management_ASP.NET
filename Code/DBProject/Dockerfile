# Dockerfile for Hospital Management System - Azure Container Apps Ready
# This enables deployment to Azure Container Apps or Azure Kubernetes Service

# Build stage
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY ["Clinic Management System.csproj", "./"]
RUN nuget restore "Clinic Management System.csproj"

# Copy everything else and build
COPY . .
RUN msbuild "Clinic Management System.csproj" /p:Configuration=Release /p:OutputPath=/app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
WORKDIR /inetpub/wwwroot

# Copy published app
COPY --from=build /app/out .

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD powershell -command \
  try { \
    $response = Invoke-WebRequest -Uri http://localhost:80 -UseBasicParsing -TimeoutSec 2; \
    if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } \
  } catch { exit 1 }

# Set environment variables (will be overridden by Azure)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV SQL_CONNECTION_STRING=""
ENV APPINSIGHTS_INSTRUMENTATIONKEY=""

# Configure IIS
RUN powershell -NoProfile -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'
