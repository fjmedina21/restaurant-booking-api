using System.Net;
using RestaurantBooking.API.Models.ApiResponse;

namespace RestaurantBooking.API.Middlewares
{
    public class GlobalErrorHandler(ILogger<GlobalErrorHandler> logger) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var type = HttpStatusCode.InternalServerError.ToString();
                var traceId = Guid.NewGuid().ToString();
                var message = ex.Message;

                string detail = $"{traceId} : {message}";
                logger.LogError(ex, detail);

                var error = new ApiErrorResponse(
                    statusCode: context.Response.StatusCode,
                    errormessage: new { traceId, detail }
                    );

                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }

    public static class GlobalErrorHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder builder) => builder.UseMiddleware<GlobalErrorHandler>();
    }
}
