namespace RestaurantBooking.API.Models.DTO
{
    public record TokenPayload(string UserId, string Email, string Role);
}
