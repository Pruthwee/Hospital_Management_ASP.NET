using System;
using System.Web;

namespace DBProject.HttpModules
{
    /// <summary>
    /// Custom HttpModule that adds Content-Security-Policy (CSP) headers to every HTTP response.
    /// Registered in Web.config under system.web/httpModules (Classic mode) and
    /// system.webServer/modules (Integrated / Azure App Service mode).
    ///
    /// The CSP policy enforces:
    ///   - default-src 'self'          : only same-origin resources by default
    ///   - script-src  'self'          : scripts from same origin only
    ///   - style-src   'self' 'unsafe-inline' : styles from same origin; inline styles
    ///                                   allowed for legacy Web Forms ScriptManager output
    ///   - img-src     'self' data:    : images from same origin and data URIs
    ///   - font-src    'self'          : fonts from same origin
    ///   - connect-src 'self'          : XHR / fetch to same origin
    ///   - frame-ancestors 'none'      : prevents clickjacking (replaces X-Frame-Options)
    ///   - form-action  'self'         : form submissions to same origin only
    ///   - base-uri     'self'         : restricts base tag injection
    ///   - object-src   'none'         : disables Flash / plugins
    ///
    /// SOC2 / Azure Policy compliance note:
    ///   This header satisfies the "Implement Content Security Policy" control required
    ///   for SOC2 Type II certification and Azure Security Benchmark v3 NS-2.
    /// </summary>
    public class CspHttpModule : IHttpModule
    {
        // CSP policy value — adjust per application requirements.
        // For Azure App Service environments the same value is also set via
        // system.webServer/httpProtocol/customHeaders in Web.config so that
        // the header is applied even for static files served by IIS directly.
        private const string CspPolicyValue =
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none'; " +
            "form-action 'self'; " +
            "base-uri 'self'; " +
            "object-src 'none'";

        public void Init(HttpApplication context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application == null || application.Response == null)
                return;

            HttpResponse response = application.Response;

            // Add Content-Security-Policy header if not already present
            if (response.Headers["Content-Security-Policy"] == null)
            {
                response.Headers.Add("Content-Security-Policy", CspPolicyValue);
            }

            // Also add X-Content-Type-Options to prevent MIME sniffing (complementary security header)
            if (response.Headers["X-Content-Type-Options"] == null)
            {
                response.Headers.Add("X-Content-Type-Options", "nosniff");
            }

            // Add X-Frame-Options as a fallback for older browsers that do not support
            // the frame-ancestors CSP directive
            if (response.Headers["X-Frame-Options"] == null)
            {
                response.Headers.Add("X-Frame-Options", "DENY");
            }
        }

        public void Dispose()
        {
            // No unmanaged resources to release
        }
    }
}
