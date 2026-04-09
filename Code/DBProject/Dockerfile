# Hospital Management System - Cloud-Ready Dockerfile
# Base image: .NET Framework 4.8 on Windows Server 2019 LTSC
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019

# Set working directory
WORKDIR /inetpub/wwwroot

# Copy application files
COPY . .

# Configure IIS
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name processModel.idleTimeout -Value '00:00:00'; \
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name recycling.periodicRestart.time -Value '00:00:00'; \
    Set-ItemProperty 'IIS:\Sites\Default Web Site' -Name applicationPool -Value 'DefaultAppPool'

# Expose port 80
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=60s --retries=3 \
    CMD powershell -command "try { \
        $response = Invoke-WebRequest -Uri http://localhost -UseBasicParsing -TimeoutSec 3; \
        if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } \
    } catch { exit 1 }"

# Start IIS
CMD ["powershell", "-Command", "Start-Service W3SVC; while ($true) { Start-Sleep -Seconds 3600 }"]
