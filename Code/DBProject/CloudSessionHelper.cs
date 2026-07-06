using System;
using System.Web;
using System.Web.SessionState;

namespace DBProject
{
    /// <summary>
    /// Cloud-ready session helper for Amazon ElastiCache for Redis distributed session state.
    /// 
    /// This helper wraps ASP.NET HttpSessionState to provide a consistent interface
    /// that works with both in-process (local dev) and distributed Redis session state
    /// (AWS ElastiCache for Redis via Microsoft.Web.RedisSessionStateProvider).
    /// 
    /// To enable Redis distributed session state:
    /// 1. Add NuGet package: Microsoft.Web.RedisSessionStateProvider
    /// 2. Set REDIS_CONNECTION_STRING environment variable to your ElastiCache endpoint
    /// 3. Update Web.config sessionState section:
    ///    &lt;sessionState mode="Custom" customProvider="RedisSessionStateProvider"&gt;
    ///      &lt;providers&gt;
    ///        &lt;add name="RedisSessionStateProvider"
    ///             type="Microsoft.Web.Redis.RedisSessionStateProvider"
    ///             connectionString="REDIS_CONNECTION_STRING"
    ///             throwOnError="false" /&gt;
    ///      &lt;/providers&gt;
    ///    &lt;/sessionState&gt;
    /// 
    /// This enables stateless horizontal scaling across multiple ECS tasks or EC2 instances
    /// without sticky session routing (cr-dotnet-0045, cr-dotnet-0126).
    /// </summary>
    public static class CloudSessionHelper
    {
        /// <summary>
        /// Gets a string value from distributed session state.
        /// </summary>
        public static string GetString(HttpSessionState session, string key)
        {
            if (session == null) return string.Empty;
            var val = session[key];
            return val != null ? val.ToString() : string.Empty;
        }

        /// <summary>
        /// Gets an integer value from distributed session state.
        /// </summary>
        public static int GetInt(HttpSessionState session, string key)
        {
            if (session == null) return 0;
            var val = session[key];
            if (val == null) return 0;
            if (val is int) return (int)val;
            int result;
            return int.TryParse(val.ToString(), out result) ? result : 0;
        }

        /// <summary>
        /// Sets a value in distributed session state.
        /// </summary>
        public static void Set(HttpSessionState session, string key, object value)
        {
            if (session != null)
            {
                session[key] = value;
            }
        }

        /// <summary>
        /// Clears a session key.
        /// </summary>
        public static void Clear(HttpSessionState session, string key)
        {
            if (session != null)
            {
                session.Remove(key);
            }
        }
    }
}
