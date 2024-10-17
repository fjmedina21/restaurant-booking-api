namespace RestaurantBooking.API.Models.DTO
{
    public record RoleDto(string Name, string? Description);
    public record RoleGDto(string RoleId, string Name, string? Description);
}
