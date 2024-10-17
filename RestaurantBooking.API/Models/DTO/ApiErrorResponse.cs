using Microsoft.AspNetCore.Mvc;

namespace RestaurantBooking.API.Models.DTO
{
    public record ApiErrorResponse(int StatusCode, string Error, object ErrorDetail);
}
