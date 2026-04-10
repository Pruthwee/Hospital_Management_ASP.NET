using System;
using System.Configuration;

namespace DBProject.Helpers
{
    /// <summary>
    /// Configuration Helper for Cloud-Ready Configuration Management
    /// 
    /// CLOUD READINESS:
    /// This helper class provides a centralized way to access configuration values,
    /// supporting multiple configuration sources in priority order:
    /// 1. Environment Variables (highest priority - Azure App Service settings)
    /// 2. AppSettings in Web.config
    /// 3. Default values
    /// 
    /// AZURE DEPLOYMENT:
    /// When deploying to Azure App Service, configuration values can be overridden
    /// via Application Settings without modifying Web.config or redeploying.
    /// 
    /// USAGE:
    /// var connectionString = ConfigHelper.GetConnectionString("sqlCon1");
    /// var environment = ConfigHelper.GetAppSetting("Environment", "Development");
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// Gets a connection string from configuration
        /// Priority: Environment Variable > Web.config
        /// </summary>
        /// <param name="name">Connection string name</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Connection string value</returns>
        public static string GetConnectionString(string name, string defaultValue = null)
        {
            try
            {
                // Check environment variable first (Azure App Service pattern)
                // Azure uses SQLAZURECONNSTR_ prefix for SQL Azure connection strings
                var envVarName = $"SQLAZURECONNSTR_{name}";
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                
                if (!string.IsNullOrEmpty(envValue))
                {
                    System.Diagnostics.Trace.TraceInformation($"Using connection string from environment variable: {envVarName}");
                    return envValue;
                }

                // Check standard environment variable
                envValue = Environment.GetEnvironmentVariable(name);
                if (!string.IsNullOrEmpty(envValue))
                {
                    System.Diagnostics.Trace.TraceInformation($"Using connection string from environment variable: {name}");
                    return envValue;
                }

                // Fall back to Web.config
                var connectionString = ConfigurationManager.ConnectionStrings[name];
                if (connectionString != null && !string.IsNullOrEmpty(connectionString.ConnectionString))
                {
                    System.Diagnostics.Trace.TraceInformation($"Using connection string from Web.config: {name}");
                    return connectionString.ConnectionString;
                }

                // Return default value
                if (defaultValue != null)
                {
                    System.Diagnostics.Trace.TraceWarning($"Connection string '{name}' not found, using default value");
                    return defaultValue;
                }

                throw new ConfigurationErrorsException($"Connection string '{name}' not found in configuration");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error getting connection string '{name}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets an application setting from configuration
        /// Priority: Environment Variable > Web.config AppSettings
        /// </summary>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValue">Default value if not found</param>
        /// <returns>Setting value</returns>
        public static string GetAppSetting(string key, string defaultValue = null)
        {
            try
            {
                // Check environment variable first (Azure App Service pattern)
                // Azure uses APPSETTING_ prefix for app settings
                var envVarName = $"APPSETTING_{key}";
                var envValue = Environment.GetEnvironmentVariable(envVarName);
                
                if (!string.IsNullOrEmpty(envValue))
                {
                    System.Diagnostics.Trace.TraceInformation($"Using app setting from environment variable: {envVarName}");
                    return envValue;
                }

                // Check standard environment variable
                envValue = Environment.GetEnvironmentVariable(key);
                if (!string.IsNullOrEmpty(envValue))
                {
                    System.Diagnostics.Trace.TraceInformation($"Using app setting from environment variable: {key}");
                    return envValue;
                }

                // Fall back to Web.config
                var appSetting = ConfigurationManager.AppSettings[key];
                if (!string.IsNullOrEmpty(appSetting))
                {
                    System.Diagnostics.Trace.TraceInformation($"Using app setting from Web.config: {key}");
                    return appSetting;
                }

                // Return default value
                if (defaultValue != null)
                {
                    System.Diagnostics.Trace.TraceInformation($"App setting '{key}' not found, using default value: {defaultValue}");
                    return defaultValue;
                }

                System.Diagnostics.Trace.TraceWarning($"App setting '{key}' not found and no default value provided");
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error getting app setting '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets an application setting as an integer
        /// </summary>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValue">Default value if not found or invalid</param>
        /// <returns>Setting value as integer</returns>
        public static int GetAppSettingInt(string key, int defaultValue = 0)
        {
            var value = GetAppSetting(key);
            
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (int.TryParse(value, out int result))
            {
                return result;
            }

            System.Diagnostics.Trace.TraceWarning($"App setting '{key}' value '{value}' is not a valid integer, using default: {defaultValue}");
            return defaultValue;
        }

        /// <summary>
        /// Gets an application setting as a boolean
        /// </summary>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValue">Default value if not found or invalid</param>
        /// <returns>Setting value as boolean</returns>
        public static bool GetAppSettingBool(string key, bool defaultValue = false)
        {
            var value = GetAppSetting(key);
            
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            if (bool.TryParse(value, out bool result))
            {
                return result;
            }

            // Handle common string representations
            value = value.ToLower();
            if (value == "1" || value == "yes" || value == "on")
            {
                return true;
            }
            if (value == "0" || value == "no" || value == "off")
            {
                return false;
            }

            System.Diagnostics.Trace.TraceWarning($"App setting '{key}' value '{value}' is not a valid boolean, using default: {defaultValue}");
            return defaultValue;
        }

        /// <summary>
        /// Gets the current environment name
        /// </summary>
        /// <returns>Environment name (Development, Staging, Production)</returns>
        public static string GetEnvironment()
        {
            return GetAppSetting("Environment", "Development");
        }

        /// <summary>
        /// Checks if running in development environment
        /// </summary>
        /// <returns>True if development, false otherwise</returns>
        public static bool IsDevelopment()
        {
            return GetEnvironment().Equals("Development", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if running in production environment
        /// </summary>
        /// <returns>True if production, false otherwise</returns>
        public static bool IsProduction()
        {
            return GetEnvironment().Equals("Production", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if running in staging environment
        /// </summary>
        /// <returns>True if staging, false otherwise</returns>
        public static bool IsStaging()
        {
            return GetEnvironment().Equals("Staging", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the Application Insights instrumentation key
        /// </summary>
        /// <returns>Instrumentation key or empty string</returns>
        public static string GetApplicationInsightsKey()
        {
            return GetAppSetting("ApplicationInsights:InstrumentationKey", string.Empty);
        }

        /// <summary>
        /// Gets the Redis connection string for session state
        /// </summary>
        /// <returns>Redis connection string or null</returns>
        public static string GetRedisConnectionString()
        {
            return GetAppSetting("RedisConnection");
        }

        /// <summary>
        /// Gets the session timeout in minutes
        /// </summary>
        /// <returns>Session timeout in minutes</returns>
        public static int GetSessionTimeout()
        {
            return GetAppSettingInt("SessionTimeout", 20);
        }

        /// <summary>
        /// Validates that all required configuration values are present
        /// Call this during application startup
        /// </summary>
        /// <returns>True if all required settings are present, false otherwise</returns>
        public static bool ValidateConfiguration()
        {
            bool isValid = true;

            try
            {
                // Check required connection strings
                var connectionString = GetConnectionString("sqlCon1");
                if (string.IsNullOrEmpty(connectionString))
                {
                    System.Diagnostics.Trace.TraceError("Required connection string 'sqlCon1' is missing");
                    isValid = false;
                }

                // Check environment
                var environment = GetEnvironment();
                System.Diagnostics.Trace.TraceInformation($"Running in environment: {environment}");

                // In production, ensure Application Insights is configured
                if (IsProduction())
                {
                    var aiKey = GetApplicationInsightsKey();
                    if (string.IsNullOrEmpty(aiKey))
                    {
                        System.Diagnostics.Trace.TraceWarning("Application Insights instrumentation key is not configured for production");
                    }
                }

                return isValid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Configuration validation error: {ex.Message}");
                return false;
            }
        }
    }
}
