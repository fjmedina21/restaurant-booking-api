using Microsoft.AspNetCore.Mvc;
using RestaurantBooking.API.Models.ApiResponse;
using RestaurantBooking.API.Models.DTO;
using RestaurantBooking.API.Helpers.Pagination;
using RestaurantBooking.API.Services.RoleService;

namespace RestaurantBooking.API.Controllers
{
    public class RolesController(IRoleService roleService):BaseController
    {

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams paginationParams)
        {
            ApiResponse<RoleGDto> response = await roleService.GetAllAsync(paginationParams);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string uid)
        {
            ApiResponse<RoleGDto> response = await roleService.GetByIdAsync(uid);
            return  StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RoleDto model)
        {
            ApiResponse<RoleGDto> response = await roleService.CreateAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        /*[HttpPut("{uid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(string uid, [FromBody] RoleDto model)
        {
            ApiResponse<RoleDto> response = await roleService.UpdateAsync(uid, model);
            return StatusCode(response.StatusCode, response);
        }*/

        [HttpDelete("{uid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string uid)
        {
            ApiResponse<RoleGDto> response = await roleService.DeleteAsync(uid);
            return StatusCode(response.StatusCode, response);
        }
    }
}
