using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickNote_Models.Token;
using QuickNote_Models.User;
using QuickNote_Services.Token;
using QuickNote_Services.User;

namespace QuickNote_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ITokenService _tokenService;

        public UserController(IUserService service, ITokenService tokenService)
        {
            _service = service;
            _tokenService = tokenService;
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

        [Authorize]
        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetUserById([FromRoute] int userId)
        {
            var userDetail = await _service.GetUserByIdAsync(userId);

            if(userDetail is null)
                return NotFound();

            return Ok(userDetail);
        }

        // The ~ before the route overrides the basic route configuration, since we are in the User controller but this is for Token (Added it to User controller instead of creating a separate Token controller)
        [HttpPost("~/api/Token")]
        public async Task<IActionResult> Token([FromBody] TokenRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var tokenResponse = await _tokenService.GetTokenAsync(request);

            if(tokenResponse is null)
                return BadRequest("Invalid username or password");

            return Ok(tokenResponse);
        }
    }
}