using Newtonsoft.Json;
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
    }
}
