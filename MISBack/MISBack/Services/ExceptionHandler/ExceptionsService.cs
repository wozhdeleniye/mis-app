using MISBack.Data.Models;
using Newtonsoft.Json;
using System.Net;

namespace MISBack.Services.ExceptionHandler
{
    public class ExceptionsService
    {
        private readonly RequestDelegate _next;

        public ExceptionsService(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionMessageAsync(context, ex).ConfigureAwait(false);
            }
        }

        private static Task HandleExceptionMessageAsync(HttpContext context, Exception exception)
        {
            try
            {
                var statusCode = exception.Data.Keys.Cast<string>().Single();
                var statusMessage = exception.Data[statusCode]?.ToString();
                var result = JsonConvert.SerializeObject(new ResponseModel
                {
                    status = statusCode,
                    message = statusMessage
                });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = int.Parse(statusCode);
                return context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                var result = JsonConvert.SerializeObject(new ResponseModel
                {
                    status = StatusCodes.Status500InternalServerError.ToString(),
                    message = ex.Message
                });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return context.Response.WriteAsync(result);
            }
        }
    }
}
