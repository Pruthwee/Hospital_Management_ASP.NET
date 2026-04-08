using System;
using System.Configuration;

namespace DBProject.Helpers
{
    /// <summary>
    /// Cloud-ready configuration helper that prioritizes environment variables over Web.config
    /// This follows the 12-factor app principles for cloud deployment
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Gets a configuration value, prioritizing environment variables over app settings
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Configuration value</returns>
        public static string GetSetting(string key, string defaultValue = null)
        {
            // Priority 1: Environment variable (Azure App Service Configuration)
            string value = Environment.GetEnvironmentVariable(key);
            
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Priority 2: App Settings in Web.config
            value = ConfigurationManager.AppSettings[key];
            
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Priority 3: Default value
            return defaultValue;
        }

        /// <summary>
        /// Gets a configuration value as integer
        /// </summary>
        public static int GetSettingAsInt(string key, int defaultValue = 0)
        {
            string value = GetSetting(key);
            
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets a configuration value as boolean
        /// </summary>
        public static bool GetSettingAsBool(string key, bool defaultValue = false)
        {
            string value = GetSetting(key);
            
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets connection string, prioritizing environment variables
        /// Azure App Service automatically prefixes connection strings with SQLCONNSTR_, MYSQLCONNSTR_, etc.
        /// </summary>
        public static string GetConnectionString(string name)
        {
            // Priority 1: Azure App Service connection string environment variable
            string envVarName = $"SQLCONNSTR_{name}";
            string value = Environment.GetEnvironmentVariable(envVarName);
            
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Priority 2: Custom connection string environment variable
            envVarName = $"CUSTOMCONNSTR_{name}";
            value = Environment.GetEnvironmentVariable(envVarName);
            
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            // Priority 3: Connection string from Web.config
            var connStr = ConfigurationManager.ConnectionStrings[name];
            if (connStr != null && !string.IsNullOrEmpty(connStr.ConnectionString))
            {
                return connStr.ConnectionString;
            }

            return null;
        }

        /// <summary>
        /// Gets the current environment (Development, Staging, Production)
        /// </summary>
        public static string GetEnvironment()
        {
            return GetSetting("ASPNETCORE_ENVIRONMENT", 
                   GetSetting("Environment", "Production"));
        }

        /// <summary>
        /// Checks if running in development environment
        /// </summary>
        public static bool IsDevelopment()
        {
            return GetEnvironment().Equals("Development", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if running in production environment
        /// </summary>
        public static bool IsProduction()
        {
            return GetEnvironment().Equals("Production", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets Application Insights instrumentation key
        /// </summary>
        public static string GetApplicationInsightsKey()
        {
            return GetSetting("APPINSIGHTS_INSTRUMENTATIONKEY", 
                   GetSetting("ApplicationInsightsInstrumentationKey"));
        }

        /// <summary>
        /// Gets Redis connection string for distributed caching
        /// </summary>
        public static string GetRedisConnectionString()
        {
            return GetConnectionString("RedisConnection") ?? 
                   GetSetting("RedisConnection");
        }

        /// <summary>
        /// Checks if distributed cache (Redis) is enabled
        /// </summary>
        public static bool UseDistributedCache()
        {
            return GetSettingAsBool("UseDistributedCache", false) && 
                   !string.IsNullOrEmpty(GetRedisConnectionString());
        }

        /// <summary>
        /// Checks if Application Insights is enabled
        /// </summary>
        public static bool UseApplicationInsights()
        {
            return GetSettingAsBool("UseApplicationInsights", true) && 
                   !string.IsNullOrEmpty(GetApplicationInsightsKey());
        }

        /// <summary>
        /// Gets session timeout in minutes
        /// </summary>
        public static int GetSessionTimeout()
        {
            return GetSettingAsInt("SessionTimeout", 20);
        }
    }
}
