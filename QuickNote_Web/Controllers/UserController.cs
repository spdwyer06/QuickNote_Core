using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickNote_Models.User;
using QuickNote_Services.User;

namespace QuickNote_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        // [HttpVerb("Route")]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister model)
        {
            // Checking to make sure the model is valid
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var wasAdded = await _service.RegisterUserAsync(model);
            if(wasAdded)
                return Ok("User was registered");
            
            return BadRequest("User could not be registered");
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserById([FromRoute] int userId)
        {
            var userDetail = await _service.GetUserByIdAsync(userId);

            if(userDetail is null)
                return NotFound();

            return Ok(userDetail);
        }
    }
}