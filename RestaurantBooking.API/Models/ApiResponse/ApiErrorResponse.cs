using Microsoft.AspNetCore.Mvc;

namespace RestaurantBooking.API.Models.ApiResponse
{
    public class ApiErrorResponse(int statusCode, string? error = null, object errormessage = null!) : BaseApiResponse(statusCode)
    {
        public string? ErrorType { get; set; } = error ?? DefaultMessage(statusCode);
    }
}
