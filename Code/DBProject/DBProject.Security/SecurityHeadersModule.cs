using System;
using System.Collections.Generic;
using System.Web;

namespace DBProject.Security
{
    /// <summary>
    /// HTTP module that adds Content Security Policy (CSP) and other security headers
    /// to every HTTP response. This addresses the cr-dotnet-1041 blocker for
    /// missing Content Security Policy headers.
    /// </summary>
    public class SecurityHeadersModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        private static void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;
            var response = context.Response;

            // Content Security Policy header
            // Restricts sources of content to prevent XSS and data injection attacks
            response.Headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data:; " +
                "font-src 'self'; " +
                "connect-src 'self'; " +
                "frame-ancestors 'self'; " +
                "object-src 'none'; " +
                "base-uri 'self'";

            // X-Content-Type-Options: prevent MIME-type sniffing
            response.Headers["X-Content-Type-Options"] = "nosniff";

            // X-Frame-Options: prevent clickjacking
            response.Headers["X-Frame-Options"] = "SAMEORIGIN";

            // X-XSS-Protection: enable browser XSS filtering (legacy browsers)
            response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Referrer-Policy: control how much referrer information is sent
            response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Strict-Transport-Security: enforce HTTPS
            response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        public void Dispose()
        {
            // No resources to dispose
        }
    }
}
