namespace RestaurantBooking.API.Models.DTO
{
    public record CustomerDto(string FullName, string Email, string PhoneNumber);
    public record CustomerGDto(string CustomerId, string FullName, string Email, string PhoneNumber);

}
