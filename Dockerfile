FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY Code/DBProject/Clinic\ Management\ System.csproj Code/DBProject/
COPY Code/packages/ Code/packages/
WORKDIR /app/Code/DBProject
RUN nuget restore "Clinic Management System.csproj"

# Copy everything else and build
COPY Code/ Code/
WORKDIR /app/Code/DBProject
RUN msbuild "Clinic Management System.csproj" /p:Configuration=Release /p:DeployOnBuild=true /p:PackageAsSingleFile=false /p:PackageLocation=C:\publish /m

# Final image
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.5.2-windowsservercore-ltsc2019
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR /inetpub/wwwroot
COPY --from=build C:\publish .
# Remove residual PDB and XML doc files
RUN powershell -Command "Get-ChildItem -Recurse -Include *.pdb,*.xml | Remove-Item -Force"
EXPOSE 80
