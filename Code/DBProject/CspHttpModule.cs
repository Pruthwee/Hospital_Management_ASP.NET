using System;
using System.Web;

namespace DBProject
{
    /// <summary>
    /// HTTP module that adds Content-Security-Policy and other security headers to every response.
    /// Required for cr-dotnet-1041 (Missing Content Security Policy Headers).
    /// </summary>
    public class CspHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += (sender, e) =>
            {
                var app = (HttpApplication)sender;
                var response = app.Response;

                if (!response.HeadersAdded)
                {
                    // Content Security Policy header
                    response.Headers.Add("Content-Security-Policy",
                        "default-src 'self'; " +
                        "script-src 'self' 'unsafe-inline' https://ajax.googleapis.com https://maxcdn.bootstrapcdn.com; " +
                        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://maxcdn.bootstrapcdn.com; " +
                        "img-src 'self' data: https:; " +
                        "font-src 'self' https://fonts.googleapis.com https://maxcdn.bootstrapcdn.com; " +
                        "connect-src 'self'");
                    response.Headers.Add("X-Content-Type-Options", "nosniff");
                    response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                    response.Headers.Add("X-XSS-Protection", "1; mode=block");
                    response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
                    response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
                }
            };
        }

        public void Dispose()
        {
            // No resources to dispose
        }
    }
}
