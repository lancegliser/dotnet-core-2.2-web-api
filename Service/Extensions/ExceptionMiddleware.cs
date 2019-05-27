using Service.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.Extensions {

    public class ExceptionMiddleware
    {
        protected readonly RequestDelegate _next;
        // private readonly ILoggerManager _logger;
        protected readonly IHostingEnvironment _env;
        protected readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public ExceptionMiddleware(IHostingEnvironment env, RequestDelegate next)
        {
            _env = env;
            // _logger = logger;
            _next = next;
        }
    
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
    
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unchaught exception at {Controller}.{Method}", nameof(ExceptionMiddleware), nameof(HandleExceptionAsync));
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            bool IsProduction = _env != null && _env.IsProduction();
            var apiException = new Models.ApiException()
            {
                Status = context.Response.StatusCode,
                Title = "An unhandled exception occurred",
                Detail = exception.Message,
                Exception = !IsProduction ? exception : null,
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(apiException));
        }
    }
}
