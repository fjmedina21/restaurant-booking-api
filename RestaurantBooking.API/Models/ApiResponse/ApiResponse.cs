namespace RestaurantBooking.API.Models.ApiResponse
{
    public class ApiResponse<T>(int statusCode = 200, string? message = null, List<T> data = null!, string? token= null) : BaseApiResponse(statusCode, message)
    {
        public List<T> Data { get; set; } = data;
        public string? Token { get; set; } = token;
    }
}
