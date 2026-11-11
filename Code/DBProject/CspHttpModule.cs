using System;
using System.Web;

namespace DBProject
{
    public class CspHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += OnPreSendRequestHeaders;
        }

        private void OnPreSendRequestHeaders(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            app.Response.Headers.Add("Content-Security-Policy",
                "default-src 'self'; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://maxcdn.bootstrapcdn.com https://ajax.googleapis.com; " +
                "script-src 'self' 'unsafe-inline' https://ajax.googleapis.com https://maxcdn.bootstrapcdn.com; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' https://fonts.googleapis.com https://maxcdn.bootstrapcdn.com; " +
                "connect-src 'self'; " +
                "frame-ancestors 'self';");
        }

        public void Dispose()
        {
        }
    }
}
