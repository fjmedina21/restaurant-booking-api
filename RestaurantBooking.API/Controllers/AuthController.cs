using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Services.AuthService;

namespace RestaurantBooking.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            ApiResponse<StaffGDto> response = await authService.LoginAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            string token = Request.Headers["Authorization"]!;
            ApiResponse<object> response = await authService.ChangePasswordAsync(model, token);
            return StatusCode(response.StatusCode, response);
        }
    }
}
