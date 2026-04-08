using System;
using System.Data.SqlClient;
using System.Web;
using System.Configuration;
using System.Text;

namespace DBProject.Handlers
{
    /// <summary>
    /// Health check handler for Azure App Service health monitoring
    /// Endpoint: /health
    /// Returns: 200 OK if healthy, 503 Service Unavailable if unhealthy
    /// </summary>
    public class HealthCheckHandler : IHttpHandler
    {
        public bool IsReusable => true;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            var healthStatus = new HealthStatus();

            try
            {
                // Check database connectivity
                healthStatus.DatabaseHealthy = CheckDatabaseHealth();

                // Check application status
                healthStatus.ApplicationHealthy = true;
                healthStatus.Timestamp = DateTime.UtcNow;
                healthStatus.Environment = ConfigurationManager.AppSettings["Environment"] ?? "Unknown";

                // Overall health
                healthStatus.Healthy = healthStatus.DatabaseHealthy && healthStatus.ApplicationHealthy;

                // Set response status code
                context.Response.StatusCode = healthStatus.Healthy ? 200 : 503;

                // Return JSON response
                context.Response.Write(healthStatus.ToJson());
            }
            catch (Exception ex)
            {
                healthStatus.Healthy = false;
                healthStatus.ApplicationHealthy = false;
                healthStatus.ErrorMessage = ex.Message;
                
                context.Response.StatusCode = 503;
                context.Response.Write(healthStatus.ToJson());
            }
        }

        private bool CheckDatabaseHealth()
        {
            try
            {
                // Get connection string from environment variable or config
                string connStr = Environment.GetEnvironmentVariable("SQLCONNSTR_sqlCon1");
                
                if (string.IsNullOrEmpty(connStr))
                {
                    connStr = ConfigurationManager.ConnectionStrings["sqlCon1"]?.ConnectionString;
                }

                if (string.IsNullOrEmpty(connStr))
                {
                    return false;
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    
                    using (SqlCommand cmd = new SqlCommand("SELECT 1", conn))
                    {
                        cmd.CommandTimeout = 5;
                        cmd.ExecuteScalar();
                    }
                    
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private class HealthStatus
        {
            public bool Healthy { get; set; }
            public bool DatabaseHealthy { get; set; }
            public bool ApplicationHealthy { get; set; }
            public DateTime Timestamp { get; set; }
            public string Environment { get; set; }
            public string ErrorMessage { get; set; }

            public string ToJson()
            {
                var sb = new StringBuilder();
                sb.Append("{");
                sb.AppendFormat("\"healthy\":{0},", Healthy.ToString().ToLower());
                sb.AppendFormat("\"database\":{0},", DatabaseHealthy.ToString().ToLower());
                sb.AppendFormat("\"application\":{0},", ApplicationHealthy.ToString().ToLower());
                sb.AppendFormat("\"timestamp\":\"{0}\",", Timestamp.ToString("o"));
                sb.AppendFormat("\"environment\":\"{0}\"", Environment);
                
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    sb.AppendFormat(",\"error\":\"{0}\"", ErrorMessage.Replace("\"", "\\\""));
                }
                
                sb.Append("}");
                return sb.ToString();
            }
        }
    }
}
