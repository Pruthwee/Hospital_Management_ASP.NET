using System;
using Microsoft.AspNetCore.Http;

namespace DBProject
{
    // Helpers for the ASP.NET Core session that keep the semantics of the
    // Web Forms Session["key"] casts used by the pages.
    public static class SessionExtensions
    {
        // Replacement for (int)Session[key]: throws when the value is missing or not an int
        public static int GetRequiredInt32(this ISession session, string key)
        {
            int? value = session.GetInt32(key);
            if (value == null)
            {
                throw new InvalidCastException("Session value '" + key + "' is not set to an int.");
            }
            return value.Value;
        }

        // Replacement for (string)Session[key]: null when missing
        public static string GetStringOrNull(this ISession session, string key)
        {
            return session.GetString(key);
        }
    }
}
