using customerportalapi.Entities;
using customerportalapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace customerportalapi.Loggers
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomLogFilter : ActionFilterAttribute, IExceptionFilter
    {
        private readonly IApiLogService _apiLogService;

        public CustomLogFilter(IApiLogService apiLogService)
        {
            _apiLogService = apiLogService;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.TraceIdentifier = Guid.NewGuid().ToString();

            var body = GetRawBodyString(context.HttpContext);

            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;
            var url = context.HttpContext.Request.GetEncodedUrl();
            var username = GetUserName(context.HttpContext);
            var remoteIp = GetRemoteIpAddress(context.HttpContext);

            var actionId = context.ActionDescriptor.Id;

            CustomLog("Start Process", "OnActionExecuting", "Info", context.RouteData, body, context.HttpContext.TraceIdentifier, actionId, method, path, url, remoteIp, username);
            base.OnActionExecuting(context);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            var body = GetRawBodyString(context.HttpContext, false);

            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;
            var url = context.HttpContext.Request.GetEncodedUrl();
            var username = GetUserName(context.HttpContext);
            var remoteIp = GetRemoteIpAddress(context.HttpContext);

            var actionId = context.ActionDescriptor.Id;

            CustomLog("End Process", "OnResultExecuted", "Info", context.RouteData, body, context.HttpContext.TraceIdentifier, actionId, method, path, url, remoteIp, username);
            base.OnResultExecuted(context);
        }

        public void OnException(ExceptionContext context)
        {
            Exception e = context.Exception;

            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;
            var url = context.HttpContext.Request.GetEncodedUrl();
            var username = GetUserName(context.HttpContext);
            var remoteIp = GetRemoteIpAddress(context.HttpContext);

            var actionId = context.ActionDescriptor.Id;

            CustomLog("End Process", "OnException", "Error", context.RouteData, "", context.HttpContext.TraceIdentifier, actionId, method, path, url, remoteIp, username, e.Message, e.StackTrace);
        }

        private void CustomLog(string message, string methodName, string level, RouteData routeData, string body, string traceIdentifier,
            string actionId, string method = "", string path = "", string url = "", string remoteIp = "", string username = "",
            string exceptionMessage = "", string exceptionTrace = "")
        {
            var controllerName = routeData.Values["controller"].ToString();
            var actionName = routeData.Values["action"].ToString();

            var log = new ApiLog
            {
                Id = "",
                Logged = DateTime.UtcNow,
                TraceId = traceIdentifier,
                ActionId = actionId,
                Level = level,
                Message = message,
                MethodName = methodName,
                HttpMethod = method,
                Path = path,
                Url = url,
                Controller = controllerName,
                Action = actionName,
                Body = body,
                RemoteIp = remoteIp,
                Username = username,
                ExceptionMessage = exceptionMessage,
                ExceptionTrace = exceptionTrace
            };

            _apiLogService.AddLog(log);
        }

        private static string GetRawBodyString(HttpContext httpContext, bool request = true)
        {
            var body = "";
            if (request)
            {
                var initialBody = httpContext.Request.Body;
                try
                {
                    if (httpContext.Request.ContentLength == null || !(httpContext.Request.ContentLength > 0) ||
                        !httpContext.Request.Body.CanSeek) return body;

                    httpContext.Request.EnableRewind();
                    httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, false, 8192, true))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                finally
                {
                    httpContext.Request.Body = initialBody;
                }
            }
            else
            {
                var initialBody = httpContext.Response.Body;
                try
                {
                    httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(httpContext.Response.Body, encoding: Encoding.UTF8, false, 8192, true))
                    {
                        body = reader.ReadToEnd();
                    }
                }
                finally
                {
                    httpContext.Response.Body = initialBody;
                }
            }
            return body;
        }

        private static string GetRemoteIpAddress(HttpContext httpContext)
        {
            var remoteIp = httpContext.Connection.RemoteIpAddress;
            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            return remoteIp.ToString();
        }

        private static string GetUserName(HttpContext context)
        {
            var userName = string.Empty;
            if (context?.User != null && context.User.Identity.IsAuthenticated)
            {
                userName = context.User.Identity.Name;
            }
            else
            {
                var threadPrincipal = Thread.CurrentPrincipal;
                if (threadPrincipal != null && threadPrincipal.Identity.IsAuthenticated)
                {
                    userName = threadPrincipal.Identity.Name;
                }
            }
            return userName;
        }
    }
}