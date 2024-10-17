using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Models.Enums;
using RestaurantBooking.API.Models.Pagination;
using RestaurantBooking.API.Services.ReservationService;

namespace RestaurantBooking.API.Controllers
{
    public class ReservationsController(IReservationService reservationService) : BaseController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            ApiResponse<ReservationGDto> response = await reservationService.GetAllAsync(paginationParams);
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ReservationDto model)
        {
            ApiResponse<ReservationGDto> response = await reservationService.CreateAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        /*[HttpPut("{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string uid, [FromBody] ReservationDto model)
        {
            ApiResponse<ReservationGDto> response = await tableService.UpdateAsync(uid, model);
            return StatusCode(response.StatusCode, response);
        }*/

        [HttpDelete("{uid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string uid)
        {
            ApiResponse<ReservationGDto> response = await reservationService.DeleteAsync(uid);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{uid}/modify-status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyReservationStatus(string uid, string status)
        {
            ApiResponse<ReservationGDto> response = await reservationService.ModifyReservationStatus(uid, status );
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("cancel-reservation/{reservationCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyReservationStatus(string reservationCode)
        {
            ApiResponse<ReservationGDto> response = await reservationService.CancelReservesation(reservationCode);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("modify-reservation/{reservationCode}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ModifyReservationStatus(string reservationCode, ModifyReservationDto reservation)
        {
            ApiResponse<ReservationGDto> response = await reservationService.ModifyReservesation(reservationCode, reservation);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("get-reservation/{reservationCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetReservationByCode(string reservationCode)
        {
            ApiResponse<ReservationGDto> response = await reservationService.GetReservesationByCode(reservationCode);
            return StatusCode(response.StatusCode, response);
        }
    }
}
