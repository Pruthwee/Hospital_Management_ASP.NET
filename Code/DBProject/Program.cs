using DBProject.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Telemetry (replaces Microsoft.ApplicationInsights.Web and ApplicationInsights.config)
builder.Services.AddApplicationInsightsTelemetry();

// The Web Forms pages (.aspx) live next to their folders (Admin, Doctor, Patient), so the
// Razor Pages root is the project folder itself and the page addresses stay the same.
builder.Services.AddRazorPages(options => options.RootDirectory = "/");

// ASP.NET Core session replaces the System.Web Session state
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// The connection string moved from Web.config to appsettings.json (ConnectionStrings:sqlCon1)
myDAL.Configure(builder.Configuration.GetConnectionString("sqlCon1"));

var app = builder.Build();

// The css / js / font files stay in the assets folder and are served from /assets
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "assets")),
    RequestPath = "/assets"
});

app.UseRouting();
app.UseSession();
app.MapRazorPages();

app.Run();
