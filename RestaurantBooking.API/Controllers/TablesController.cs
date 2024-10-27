using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Helpers.Pagination;
using RestaurantBooking.API.Services.TableService;

namespace RestaurantBooking.API.Controllers
{
    public class TablesController(ITableService tableService) : BaseController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            ApiResponse<TableGDto> response = await tableService.GetAllAsync(paginationParams);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string uid)
        {
            ApiResponse<TableGDto> response = await tableService.GetByIdAsync(uid);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TableDto model)
        {
            ApiResponse<TableGDto> response = await tableService.CreateAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        /*[HttpPut("{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string uid, [FromBody] TableDto model)
        {
            ApiResponse<TableDto> response = await tableService.UpdateAsync(uid, model);
            return StatusCode(response.StatusCode, response);
        }*/

        [HttpDelete("{uid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string uid)
        {
            ApiResponse<TableGDto> response = await tableService.DeleteAsync(uid);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("available-tables")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableTables([FromQuery] DateTime reservationStart, [FromQuery] DateTime reservationEnd)
        {
            ApiResponse<TableGDto> response = await tableService.GetAvailableTablesAsync(reservationStart, reservationEnd);
            return StatusCode(response.StatusCode, response);
        }
    }
}
