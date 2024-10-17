namespace RestaurantBooking.API.Models.DTO
{
    public record ReservationDto(string TableId, DateTime ReservationStart, DateTime ReservationEnd, int NumberOfPeople, string? Preferences, CustomerDto Customer);
    public record ModifyReservationDto(string TableId, DateTime ReservationStart, DateTime ReservationEnd, int NumberOfPeople, string? Preferences);
    public record ReservationGDto(string ReservationId, CustomerGDto Customer, TableGDto Table,DateTime ReservationStart, DateTime ReservationEnd, int NumberOfPeople, string Status, string ReservationCode, string? Preferences);
}
