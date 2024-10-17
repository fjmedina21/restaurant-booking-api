namespace RestaurantBooking.API.Models.DTO
{
    public class ApiResponse<T>(int statusCode = 200, string? detail = null, List<T> data = null!, string? token= null) where T : class
    {
        public bool Ok { get; set; } = statusCode < 400;
        public int StatusCode { get; set; } = statusCode;
        public string? Detail { get; set; } = detail ?? DefaultMessage(statusCode);
        public List<T> Data { get; set; } = data;
        public string? Token { get; set; } = token;

        private static string? DefaultMessage(int statusCode) => statusCode switch
        {
            200 => "Solicitud Completada",
            201 => "Recurso Creado",
            204 => "Sin Contenido",
            400 => "Solicitud Incorrecta",
            404 => "Recurso No Encontrado",
            500 => "Error de Servidor",
            _ => null
        };
    }
}
