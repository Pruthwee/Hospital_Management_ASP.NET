<%@ WebHandler Language="C#" Class="DBProject.HealthHandler" %>

using System;
using System.Web;

namespace DBProject
{
    public class HealthHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.Write("{\"status\":\"healthy\"}");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
