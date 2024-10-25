﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RestaurantBooking.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
    }
}
