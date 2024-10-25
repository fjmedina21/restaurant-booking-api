using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestaurantBooking.API.Data;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTOs.ThirdPartyServices;

namespace RestaurantBooking.API.Helpers
{
    public static class Validations
    {
        public async static Task<ApiResponse<CedulaValidationResponse>> ValidateCedula(string cedula)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync($"https://api.digital.gob.do/v3/cedulas/{cedula}/validate");

            response.EnsureSuccessStatusCode();

            string data = await response.Content.ReadAsStringAsync();
            var deserializedObject = JsonConvert.DeserializeObject<CedulaValidationResponse>(data)!;

            if (!deserializedObject.Valid) return new ApiResponse<CedulaValidationResponse>(statusCode: (int)response.StatusCode, message: deserializedObject.Message);

            return new ApiResponse<CedulaValidationResponse>();
        }

        public async static Task<bool> TableExist(string uid, RestaurantBookingContext dbContext)
        {
            return await dbContext.Tables
                .Where(e => !e.IsDeleted).AnyAsync(e => e.TableId == uid);
        }

        public async static Task<bool> ReservationExist(string uid, RestaurantBookingContext dbContext)
        {
            return await dbContext.Reservations
                .Where(e => !e.IsDeleted).AnyAsync(e => e.ReservationId == uid);
        }

        public async static Task<bool> UserExist(string uid, RestaurantBookingContext dbContext)
        {
            return await dbContext.RestaurantStaff
                .Where(e => !e.IsDeleted).AnyAsync(e => e.StaffId == uid);
        }
    }
}
