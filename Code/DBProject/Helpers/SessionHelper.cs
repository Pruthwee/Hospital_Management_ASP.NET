using System;
using System.Web;

namespace DBProject.Helpers
{
    /// <summary>
    /// Cloud-ready session management helper
    /// CLOUD READINESS NOTES:
    /// - Provides abstraction layer for session state
    /// - Can be easily migrated to use distributed cache (Redis/Memorystore)
    /// - Supports JWT token-based authentication for stateless scenarios
    /// - Compatible with GKE horizontal scaling
    /// 
    /// MIGRATION PATH:
    /// 1. Current: Uses ASP.NET Session (requires sticky sessions in GKE)
    /// 2. Phase 1: Configure session state to use Memorystore Redis
    /// 3. Phase 2: Migrate to JWT tokens with claims-based authentication
    /// 
    /// GKE DEPLOYMENT CONFIGURATION:
    /// - Option A: Enable session affinity in GKE Ingress (short-term)
    /// - Option B: Configure Memorystore Redis for distributed sessions (recommended)
    /// - Option C: Migrate to stateless JWT authentication (long-term)
    /// </summary>
    public static class SessionHelper
    {
        // Session key constants
        private const string USER_ID_KEY = "idoriginal";
        private const string USER_TYPE_KEY = "userType";
        private const string USER_NAME_KEY = "userName";

        /// <summary>
        /// Gets the current user ID from session
        /// CLOUD READY: Can be migrated to read from JWT claims
        /// </summary>
        public static int? GetUserId()
        {
            if (HttpContext.Current?.Session?[USER_ID_KEY] != null)
            {
                var sessionValue = HttpContext.Current.Session[USER_ID_KEY];
                if (sessionValue is int)
                {
                    return (int)sessionValue;
                }
                else if (int.TryParse(sessionValue.ToString(), out int userId))
                {
                    return userId;
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the current user ID in session
        /// CLOUD READY: Can be migrated to JWT token generation
        /// </summary>
        public static void SetUserId(int userId)
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session[USER_ID_KEY] = userId;
            }
        }

        /// <summary>
        /// Gets the current user type from session
        /// CLOUD READY: Can be migrated to read from JWT claims
        /// </summary>
        public static int? GetUserType()
        {
            if (HttpContext.Current?.Session?[USER_TYPE_KEY] != null)
            {
                return (int)HttpContext.Current.Session[USER_TYPE_KEY];
            }
            return null;
        }

        /// <summary>
        /// Sets the current user type in session
        /// CLOUD READY: Can be migrated to JWT token generation
        /// </summary>
        public static void SetUserType(int userType)
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session[USER_TYPE_KEY] = userType;
            }
        }

        /// <summary>
        /// Gets the current user name from session
        /// CLOUD READY: Can be migrated to read from JWT claims
        /// </summary>
        public static string GetUserName()
        {
            if (HttpContext.Current?.Session?[USER_NAME_KEY] != null)
            {
                return HttpContext.Current.Session[USER_NAME_KEY].ToString();
            }
            return null;
        }

        /// <summary>
        /// Sets the current user name in session
        /// CLOUD READY: Can be migrated to JWT token generation
        /// </summary>
        public static void SetUserName(string userName)
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session[USER_NAME_KEY] = userName;
            }
        }

        /// <summary>
        /// Clears all session data (logout)
        /// CLOUD READY: Can be migrated to JWT token invalidation
        /// </summary>
        public static void ClearSession()
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
            }
        }

        /// <summary>
        /// Checks if user is authenticated
        /// CLOUD READY: Can be migrated to JWT token validation
        /// </summary>
        public static bool IsAuthenticated()
        {
            return GetUserId().HasValue;
        }

        /// <summary>
        /// Validates user session and redirects to login if not authenticated
        /// CLOUD READY: Can be migrated to JWT token validation
        /// </summary>
        public static void RequireAuthentication(string loginUrl = "~/SignUp.aspx")
        {
            if (!IsAuthenticated())
            {
                HttpContext.Current.Response.Redirect(loginUrl);
            }
        }

        /// <summary>
        /// Gets a generic session value
        /// CLOUD READY: Provides abstraction for future migration
        /// </summary>
        public static T GetSessionValue<T>(string key)
        {
            if (HttpContext.Current?.Session?[key] != null)
            {
                return (T)HttpContext.Current.Session[key];
            }
            return default(T);
        }

        /// <summary>
        /// Sets a generic session value
        /// CLOUD READY: Provides abstraction for future migration
        /// </summary>
        public static void SetSessionValue<T>(string key, T value)
        {
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }
    }
}
