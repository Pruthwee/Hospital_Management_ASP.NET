using System;
using System.Web;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace DBProject.CloudInfrastructure
{
    /// <summary>
    /// Cloud-ready distributed session manager using Azure Cache for Redis
    /// Replaces IIS InProc session state with distributed cache for horizontal scaling
    /// </summary>
    public class DistributedSessionManager
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            // Get Redis connection string from environment variable for cloud deployment
            string redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") 
                ?? "localhost:6379"; // Fallback for local development
            
            var options = ConfigurationOptions.Parse(redisConnectionString);
            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 5000;
            options.SyncTimeout = 5000;
            
            return ConnectionMultiplexer.Connect(options);
        });

        private static ConnectionMultiplexer Connection => lazyConnection.Value;
        private static IDatabase Cache => Connection.GetDatabase();

        /// <summary>
        /// Set a session value in distributed cache
        /// </summary>
        public static void SetSessionValue(string key, object value, TimeSpan? expiry = null)
        {
            try
            {
                string sessionId = GetSessionId();
                string fullKey = $"session:{sessionId}:{key}";
                string serializedValue = JsonConvert.SerializeObject(value);
                
                // Default expiry of 20 minutes (standard session timeout)
                TimeSpan expiryTime = expiry ?? TimeSpan.FromMinutes(20);
                
                Cache.StringSet(fullKey, serializedValue, expiryTime);
            }
            catch (Exception ex)
            {
                // Log error and fallback to in-memory session for development
                System.Diagnostics.Trace.TraceError($"Redis session error: {ex.Message}");
                
                // Fallback to HttpContext.Current.Session if Redis is unavailable
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[key] = value;
                }
            }
        }

        /// <summary>
        /// Get a session value from distributed cache
        /// </summary>
        public static T GetSessionValue<T>(string key, T defaultValue = default(T))
        {
            try
            {
                string sessionId = GetSessionId();
                string fullKey = $"session:{sessionId}:{key}";
                
                string serializedValue = Cache.StringGet(fullKey);
                
                if (string.IsNullOrEmpty(serializedValue))
                {
                    // Try fallback to in-memory session
                    if (HttpContext.Current?.Session != null && HttpContext.Current.Session[key] != null)
                    {
                        return (T)HttpContext.Current.Session[key];
                    }
                    return defaultValue;
                }
                
                return JsonConvert.DeserializeObject<T>(serializedValue);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Redis session error: {ex.Message}");
                
                // Fallback to HttpContext.Current.Session
                if (HttpContext.Current?.Session != null && HttpContext.Current.Session[key] != null)
                {
                    return (T)HttpContext.Current.Session[key];
                }
                
                return defaultValue;
            }
        }

        /// <summary>
        /// Remove a session value from distributed cache
        /// </summary>
        public static void RemoveSessionValue(string key)
        {
            try
            {
                string sessionId = GetSessionId();
                string fullKey = $"session:{sessionId}:{key}";
                
                Cache.KeyDelete(fullKey);
                
                // Also remove from in-memory session
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session.Remove(key);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Redis session error: {ex.Message}");
            }
        }

        /// <summary>
        /// Clear all session values for current session
        /// </summary>
        public static void ClearSession()
        {
            try
            {
                string sessionId = GetSessionId();
                string pattern = $"session:{sessionId}:*";
                
                // Note: In production, use Redis SCAN instead of KEYS for better performance
                var server = Connection.GetServer(Connection.GetEndPoints()[0]);
                var keys = server.Keys(pattern: pattern);
                
                foreach (var key in keys)
                {
                    Cache.KeyDelete(key);
                }
                
                // Also clear in-memory session
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session.Clear();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Redis session error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get or create session ID
        /// </summary>
        private static string GetSessionId()
        {
            if (HttpContext.Current?.Session != null)
            {
                return HttpContext.Current.Session.SessionID;
            }
            
            // For scenarios without HttpContext (background jobs, etc.)
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Check if Redis is available
        /// </summary>
        public static bool IsRedisAvailable()
        {
            try
            {
                return Connection.IsConnected;
            }
            catch
            {
                return false;
            }
        }
    }
}
