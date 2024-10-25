namespace RestaurantBooking.API.Models.DTO
{
    public record TokenPayload(string UserId, string FirstName, string LastName, string Email);
}
