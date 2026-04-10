using System;
using System.Web;

namespace DBProject.Helpers
{
    /// <summary>
    /// Session State Helper for Cloud-Ready Session Management
    /// 
    /// CLOUD READINESS:
    /// This helper class provides a centralized way to manage session state,
    /// making it easier to migrate from InProc to distributed session state
    /// (Azure Cache for Redis or SQL Server) without changing application code.
    /// 
    /// USAGE:
    /// Instead of: Session["key"] = value;
    /// Use: SessionHelper.Set("key", value);
    /// 
    /// Instead of: var value = (Type)Session["key"];
    /// Use: var value = SessionHelper.Get<Type>("key");
    /// </summary>
    public static class SessionHelper
    {
        // Session key constants for type safety
        public const string USER_ID = "idoriginal";
        public const string USER_TYPE = "userType";
        public const string USER_NAME = "userName";
        public const string DOCTOR_ID = "doctorId";
        public const string PATIENT_ID = "patientId";
        public const string ADMIN_ID = "adminId";

        /// <summary>
        /// Gets a value from session state with type safety
        /// </summary>
        /// <typeparam name="T">Type of the value to retrieve</typeparam>
        /// <param name="key">Session key</param>
        /// <param name="defaultValue">Default value if key doesn't exist</param>
        /// <returns>Value from session or default value</returns>
        public static T Get<T>(string key, T defaultValue = default(T))
        {
            try
            {
                if (HttpContext.Current?.Session == null)
                {
                    System.Diagnostics.Trace.TraceWarning($"Session is null when accessing key: {key}");
                    return defaultValue;
                }

                var value = HttpContext.Current.Session[key];
                
                if (value == null)
                {
                    return defaultValue;
                }

                // Handle type conversion
                if (value is T)
                {
                    return (T)value;
                }

                // Try to convert
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (InvalidCastException)
                {
                    System.Diagnostics.Trace.TraceWarning($"Cannot convert session value for key '{key}' to type {typeof(T).Name}");
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error getting session value for key '{key}': {ex.Message}");
                return defaultValue;
            }
        }

        /// <summary>
        /// Sets a value in session state
        /// </summary>
        /// <typeparam name="T">Type of the value to store</typeparam>
        /// <param name="key">Session key</param>
        /// <param name="value">Value to store</param>
        public static void Set<T>(string key, T value)
        {
            try
            {
                if (HttpContext.Current?.Session == null)
                {
                    System.Diagnostics.Trace.TraceWarning($"Session is null when setting key: {key}");
                    return;
                }

                HttpContext.Current.Session[key] = value;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error setting session value for key '{key}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Removes a value from session state
        /// </summary>
        /// <param name="key">Session key to remove</param>
        public static void Remove(string key)
        {
            try
            {
                if (HttpContext.Current?.Session == null)
                {
                    return;
                }

                HttpContext.Current.Session.Remove(key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error removing session value for key '{key}': {ex.Message}");
            }
        }

        /// <summary>
        /// Clears all session state
        /// </summary>
        public static void Clear()
        {
            try
            {
                if (HttpContext.Current?.Session == null)
                {
                    return;
                }

                HttpContext.Current.Session.Clear();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error clearing session: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a session key exists
        /// </summary>
        /// <param name="key">Session key to check</param>
        /// <returns>True if key exists, false otherwise</returns>
        public static bool Exists(string key)
        {
            try
            {
                if (HttpContext.Current?.Session == null)
                {
                    return false;
                }

                return HttpContext.Current.Session[key] != null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError($"Error checking session key '{key}': {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the current user ID from session
        /// </summary>
        /// <returns>User ID or 0 if not found</returns>
        public static int GetUserId()
        {
            return Get<int>(USER_ID, 0);
        }

        /// <summary>
        /// Sets the current user ID in session
        /// </summary>
        /// <param name="userId">User ID to store</param>
        public static void SetUserId(int userId)
        {
            Set(USER_ID, userId);
        }

        /// <summary>
        /// Gets the current user type from session
        /// </summary>
        /// <returns>User type (1=Patient, 2=Doctor, 3=Admin) or 0 if not found</returns>
        public static int GetUserType()
        {
            return Get<int>(USER_TYPE, 0);
        }

        /// <summary>
        /// Sets the current user type in session
        /// </summary>
        /// <param name="userType">User type (1=Patient, 2=Doctor, 3=Admin)</param>
        public static void SetUserType(int userType)
        {
            Set(USER_TYPE, userType);
        }

        /// <summary>
        /// Checks if user is authenticated
        /// </summary>
        /// <returns>True if user is authenticated, false otherwise</returns>
        public static bool IsAuthenticated()
        {
            return GetUserId() > 0;
        }

        /// <summary>
        /// Checks if current user is a patient
        /// </summary>
        /// <returns>True if user is a patient, false otherwise</returns>
        public static bool IsPatient()
        {
            return GetUserType() == 1;
        }

        /// <summary>
        /// Checks if current user is a doctor
        /// </summary>
        /// <returns>True if user is a doctor, false otherwise</returns>
        public static bool IsDoctor()
        {
            return GetUserType() == 2;
        }

        /// <summary>
        /// Checks if current user is an admin
        /// </summary>
        /// <returns>True if user is an admin, false otherwise</returns>
        public static bool IsAdmin()
        {
            return GetUserType() == 3;
        }

        /// <summary>
        /// Logs out the current user by clearing session
        /// </summary>
        public static void Logout()
        {
            Clear();
        }
    }
}
