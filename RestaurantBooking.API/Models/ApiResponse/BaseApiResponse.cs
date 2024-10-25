using System.Net;

namespace RestaurantBooking.API.Models.ApiResponse
{
	public abstract class BaseApiResponse(int statusCode = StatusCodes.Status200OK, string? message = null)
	{
		public bool Success { get; set; } = statusCode < 400;
		public int StatusCode { get; } = statusCode;
		public string? Message { get; set; } = message ?? DefaultMessage(statusCode);


		protected static string? DefaultMessage(int statusCode) => statusCode switch
		{
			StatusCodes.Status200OK => HttpStatusCode.OK.ToString(),
			StatusCodes.Status201Created => HttpStatusCode.Created.ToString(),
			StatusCodes.Status204NoContent => HttpStatusCode.NotFound.ToString(),
			StatusCodes.Status400BadRequest => HttpStatusCode.BadRequest.ToString(),
			StatusCodes.Status401Unauthorized => HttpStatusCode.Unauthorized.ToString(),
			StatusCodes.Status403Forbidden => HttpStatusCode.Forbidden.ToString(),
			StatusCodes.Status404NotFound => HttpStatusCode.NotFound.ToString(),
			StatusCodes.Status500InternalServerError => HttpStatusCode.InternalServerError.ToString(),
			_ => null
		};
	}
}
