using System;
using System.Web;

namespace DBProject.Helpers
{
    /// <summary>
    /// Cloud-ready session management helper
    /// Provides abstraction for session state that can be easily migrated to distributed cache (Azure Redis)
    /// </summary>
    public static class SessionHelper
    {
        private const string USER_ID_KEY = "idoriginal";
        private const string USER_TYPE_KEY = "userType";
        private const string USER_NAME_KEY = "userName";

        /// <summary>
        /// Gets the current HTTP context session
        /// </summary>
        private static HttpSessionState Session
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    throw new InvalidOperationException("HttpContext is not available");
                }
                return HttpContext.Current.Session;
            }
        }

        /// <summary>
        /// Gets or sets the current user ID
        /// </summary>
        public static int? UserId
        {
            get
            {
                var value = Session[USER_ID_KEY];
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    return null;
                
                if (int.TryParse(value.ToString(), out int id))
                    return id;
                
                return null;
            }
            set
            {
                Session[USER_ID_KEY] = value?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the current user type (1=Patient, 2=Doctor, 3=Admin)
        /// </summary>
        public static int? UserType
        {
            get
            {
                var value = Session[USER_TYPE_KEY];
                if (value == null)
                    return null;
                
                if (int.TryParse(value.ToString(), out int type))
                    return type;
                
                return null;
            }
            set
            {
                Session[USER_TYPE_KEY] = value;
            }
        }

        /// <summary>
        /// Gets or sets the current user name
        /// </summary>
        public static string UserName
        {
            get { return Session[USER_NAME_KEY] as string; }
            set { Session[USER_NAME_KEY] = value; }
        }

        /// <summary>
        /// Checks if a user is currently logged in
        /// </summary>
        public static bool IsAuthenticated
        {
            get { return UserId.HasValue && UserId.Value > 0; }
        }

        /// <summary>
        /// Clears all session data (logout)
        /// </summary>
        public static void Clear()
        {
            Session.Clear();
            Session.Abandon();
        }

        /// <summary>
        /// Sets user session data after successful login
        /// </summary>
        public static void SetUserSession(int userId, int userType, string userName = null)
        {
            UserId = userId;
            UserType = userType;
            UserName = userName;
        }

        /// <summary>
        /// Gets a custom session value
        /// </summary>
        public static T GetValue<T>(string key, T defaultValue = default(T))
        {
            var value = Session[key];
            if (value == null)
                return defaultValue;

            try
            {
                return (T)value;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Sets a custom session value
        /// </summary>
        public static void SetValue(string key, object value)
        {
            Session[key] = value;
        }

        /// <summary>
        /// Removes a custom session value
        /// </summary>
        public static void RemoveValue(string key)
        {
            Session.Remove(key);
        }
    }
}
