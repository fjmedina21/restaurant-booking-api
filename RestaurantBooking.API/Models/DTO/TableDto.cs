namespace RestaurantBooking.API.Models.DTO
{
    public record TableDto(string Name,int Capacity);
    public record TableGDto(string TableId,string Name, int Capacity);
}
