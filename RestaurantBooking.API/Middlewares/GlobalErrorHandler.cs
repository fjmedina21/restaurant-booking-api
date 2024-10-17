using System.Net;
using RestaurantBooking.API.Models;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.DTOs;

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
                var detail = ex.Message;

                string message = $"{traceId} : {detail}";
                logger.LogError(ex, message);

                var error = new ApiErrorResponse(
                    StatusCode: context.Response.StatusCode,
                    Error: type,
                    ErrorDetail: new { traceId, detail });

                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }

    public static class GlobalErrorHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder builder) => builder.UseMiddleware<GlobalErrorHandler>();
    }
}
