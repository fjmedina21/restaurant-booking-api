using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.API.Helpers;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Helpers.Pagination;
using RestaurantBooking.API.Services.RestaurantStaffService;

namespace RestaurantBooking.API.Controllers
{
    public class RestaurantStaffController(IRestaurantStaffService restaurantStaffService):BaseController
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            ApiResponse<StaffGDto> response = await restaurantStaffService.GetAllAsync(paginationParams);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string uid)
        {
            ApiResponse<StaffGDto> response = await restaurantStaffService.GetByIdAsync(uid);
            return  StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [ValidateModel]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterStaffDto model)
        {
            ApiResponse<StaffGDto> response = await restaurantStaffService.CreateAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        /*[HttpPut("{uid}")]
        [ValidateModel]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string uid, [FromBody] RegisterStaffDto model)
        {
            ApiResponse<StaffGDto> response = await restaurantStaffService.UpdateAsync(uid, model);
            return StatusCode(response.StatusCode, response);
        }*/

        [HttpDelete("{uid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string uid)
        {
            ApiResponse<StaffGDto> response = await restaurantStaffService.DeleteAsync(uid);
            return StatusCode(response.StatusCode, response);
        }
    }
}
