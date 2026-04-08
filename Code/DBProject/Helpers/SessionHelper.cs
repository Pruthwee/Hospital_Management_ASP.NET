using System;
using System.Web;

namespace DBProject.Helpers
{
    /// <summary>
    /// Cloud-ready session state helper for managing user sessions
    /// This helper provides a centralized way to manage session state
    /// and makes it easier to migrate to distributed session state (Redis) later
    /// </summary>
    public static class SessionHelper
    {
        // Session key constants
        private const string USER_ID_KEY = "idoriginal";
        private const string USER_TYPE_KEY = "userType";
        private const string USER_NAME_KEY = "userName";

        /// <summary>
        /// Gets or sets the current user ID
        /// </summary>
        public static int? UserId
        {
            get
            {
                var value = HttpContext.Current?.Session?[USER_ID_KEY];
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return null;
                
                if (int.TryParse(value.ToString(), out int userId))
                    return userId;
                
                return null;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[USER_ID_KEY] = value?.ToString() ?? "";
                }
            }
        }

        /// <summary>
        /// Gets or sets the current user type (1=Patient, 2=Doctor, 3=Admin)
        /// </summary>
        public static int? UserType
        {
            get
            {
                var value = HttpContext.Current?.Session?[USER_TYPE_KEY];
                if (value == null)
                    return null;
                
                if (int.TryParse(value.ToString(), out int userType))
                    return userType;
                
                return null;
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[USER_TYPE_KEY] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the current user name
        /// </summary>
        public static string UserName
        {
            get
            {
                return HttpContext.Current?.Session?[USER_NAME_KEY]?.ToString();
            }
            set
            {
                if (HttpContext.Current?.Session != null)
                {
                    HttpContext.Current.Session[USER_NAME_KEY] = value;
                }
            }
        }

        /// <summary>
        /// Checks if user is authenticated
        /// </summary>
        public static bool IsAuthenticated
        {
            get
            {
                return UserId.HasValue && UserId.Value > 0;
            }
        }

        /// <summary>
        /// Checks if current user is a patient
        /// </summary>
        public static bool IsPatient
        {
            get
            {
                return UserType.HasValue && UserType.Value == 1;
            }
        }

        /// <summary>
        /// Checks if current user is a doctor
        /// </summary>
        public static bool IsDoctor
        {
            get
            {
                return UserType.HasValue && UserType.Value == 2;
            }
        }

        /// <summary>
        /// Checks if current user is an admin
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                return UserType.HasValue && UserType.Value == 3;
            }
        }

        /// <summary>
        /// Clears all session data (logout)
        /// </summary>
        public static void Clear()
        {
            HttpContext.Current?.Session?.Clear();
        }

        /// <summary>
        /// Abandons the current session
        /// </summary>
        public static void Abandon()
        {
            HttpContext.Current?.Session?.Abandon();
        }

        /// <summary>
        /// Sets user session after successful login
        /// </summary>
        public static void SetUserSession(int userId, int userType, string userName = null)
        {
            UserId = userId;
            UserType = userType;
            UserName = userName;
        }

        /// <summary>
        /// Validates that user is authenticated, redirects to login if not
        /// </summary>
        public static void RequireAuthentication(string loginUrl = "~/SignUp.aspx")
        {
            if (!IsAuthenticated)
            {
                HttpContext.Current?.Response.Redirect(loginUrl);
            }
        }

        /// <summary>
        /// Validates that user has specific role, redirects if not
        /// </summary>
        public static void RequireRole(int requiredUserType, string unauthorizedUrl = "~/SignUp.aspx")
        {
            if (!IsAuthenticated || UserType != requiredUserType)
            {
                HttpContext.Current?.Response.Redirect(unauthorizedUrl);
            }
        }
    }
}
