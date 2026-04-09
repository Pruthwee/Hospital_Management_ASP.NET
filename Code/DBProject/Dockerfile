# Dockerfile for Hospital Management System
# Windows Container for .NET Framework 4.8 Application

# Use official Microsoft ASP.NET image
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019

# Set working directory
WORKDIR /inetpub/wwwroot

# Copy application files
COPY . .

# Environment variables (will be overridden at runtime)
ENV DB_CONNECTION_STRING="" \
    REDIS_HOST="" \
    REDIS_PORT="6379" \
    REDIS_ACCESS_KEY="" \
    APP_INSIGHTS_KEY=""

# Expose port 80
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD powershell -command \
    try { \
        $response = Invoke-WebRequest -Uri http://localhost:80 -UseBasicParsing -TimeoutSec 2; \
        if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } \
    } catch { exit 1 }

# Start IIS
ENTRYPOINT ["C:\\ServiceMonitor.exe", "w3svc"]
