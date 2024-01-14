namespace MISBack.Services.ExceptionHandler
{
    public static class MiddlewareExtensions
    {
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionsService>();
        }
    }
}
