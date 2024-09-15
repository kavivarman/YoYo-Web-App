using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using YoYo_Web_App.Models.ErrorDetails;

namespace YoYo_Web_App.MiddlewareExtensions
{
    public static class ConfigureExceptionHandlerExtensions
    {
        public static void UseConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var response = httpContext.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                _logger.LogError($"Method name: {new StackTrace(ex).GetFrame(0).GetMethod().Name}, Error: {ex?.Message}");
                await response.WriteAsync(new ErrorDetails()
                {
                    Message = "Internal Server Error",
                    StatusCode = response.StatusCode
                }.ToString());
            }
        }
    }
}
