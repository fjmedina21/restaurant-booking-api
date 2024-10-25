namespace RestaurantBooking.API.Models.DTO
{
    public record RegisterStaffDto(string FirstName, string LastName, string Email, string Password);
    public record StaffGDto(string StaffId, string FirstName, string LastName, string Email);
}
