# Clinic Management System - .NET 8

A fully migrated ASP.NET Web Forms application to .NET 8 using Clean Architecture principles.

## Architecture

This solution follows Clean Architecture with four layers:

- **Domain** (`src/ClinicManagement.Domain`) - Entities, interfaces, enums, exceptions
- **Application** (`src/ClinicManagement.Application`) - Business logic, services, DTOs, AutoMapper
- **Infrastructure** (`src/ClinicManagement.Infrastructure`) - EF Core, repositories, data access
- **Web** (`src/ClinicManagement.Web`) - Razor Pages, view models, static files

## Prerequisites

- .NET 8 SDK
- SQL Server (or SQL Server Express)

## Setup

1. Update the connection string in `src/ClinicManagement.Web/appsettings.json`
2. Run database migrations or use the existing SQL scripts in `Database Files/`
3. Run the application: `dotnet run --project src/ClinicManagement.Web`

## Building

```bash
dotnet restore ClinicManagement.sln
dotnet build ClinicManagement.sln
```

## Testing

```bash
dotnet test ClinicManagement.sln
```

## Migration Notes

- Migrated from ASP.NET Web Forms 4.5.2 to .NET 8
- Replaced ADO.NET with Entity Framework Core 8.0.0
- Replaced Web.config with appsettings.json
- Replaced Global.asax with Program.cs
- Replaced Master Pages with Razor Layout Pages
- Replaced .aspx pages with Razor Pages
- Replaced System.Web with ASP.NET Core equivalents
- Added Serilog for structured logging
- Implemented Clean Architecture with DI throughout
