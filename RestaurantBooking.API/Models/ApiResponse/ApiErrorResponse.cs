using Microsoft.AspNetCore.Mvc;

namespace RestaurantBooking.API.Models.ApiResponse
{
    public class ApiErrorResponse(int statusCode, string? error = null, object errormessage = null!) : BaseApiResponse(statusCode)
    {
        public string? Error { get; set; } = error ?? DefaultMessage(statusCode);
        public object Errormessage { get; set; } = errormessage;
    }
}
