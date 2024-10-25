using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.API.Helpers.Pagination;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Services.ReservationService;

namespace RestaurantBooking.API.Controllers
{
    public class ReservationsController(IReservationService reservationService) : BaseController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams, [FromQuery] string? status) {
            ApiResponse<ReservationGDto> response = await reservationService.GetAllAsync(paginationParams, status);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string uid)
        {
            ApiResponse<ReservationGDto> response = await reservationService.GetByIdAsync(uid);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ReservationDto model)
        {
            ApiResponse<ReservationGDto> response = await reservationService.CreateAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{uid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string uid)
        {
            ApiResponse<ReservationGDto> response = await reservationService.DeleteAsync(uid);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{uid}/change-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyReservationStatus(string uid, string status)
        {
            ApiResponse<ReservationGDto> response = await reservationService.ChangeReservationStatusAsync(uid, status );
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{reservationCode}/cancel")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelReservesation(string reservationCode)
        {
            ApiResponse<ReservationGDto> response = await reservationService.CancelReservesationAsync(reservationCode);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{reservationCode}/edit")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyReservationStatus(string reservationCode, ModifyReservationDto reservation)
        {
            ApiResponse<ReservationGDto> response = await reservationService.ModifyReservesationAsync(reservationCode, reservation);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{reservationCode}/info")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReservationByCode(string reservationCode)
        {
            ApiResponse<ReservationGDto> response = await reservationService.GetReservesationByCodeAsync(reservationCode);
            return StatusCode(response.StatusCode, response);
        }
    }
}
