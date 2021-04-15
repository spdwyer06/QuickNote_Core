using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickNote_Data;
using QuickNote_Data.Entities;
using QuickNote_Models.Note;
using QuickNote_Services.Note;

namespace QuickNote_Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _service;
        //private readonly UserManager<UserEntity> _userManager;

        public NoteController(INoteService service)
        {
            _service = service;
        }

        //  public NoteController(UserManager<UserEntity> userManager)
        // {
        //     _userManager = userManager;
        // }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreate model)
        {
            // One way to get User Id
            var id = User.FindFirst("Id").Value;
            var userId = User.Claims.FirstOrDefault(user => user.Type.ToLower().Contains("id")).Value;

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var wasCreated = await _service.CreateNoteAsync(model);

            //var service = CreateNoteService();

            //var wasCreated = await service.CreateNoteAsync(model);

            if(wasCreated)
                return Ok("Your note was created");

            return BadRequest("Your note could not be created");
        }

        [HttpGet("All")]
        public IActionResult GetAllNotes()
        {
            var notes = _service.GetAllNotes();

            return Ok(notes);
        }

        [HttpGet("{noteId:int}")]
        public async Task<IActionResult> GetNoteByNoteId([FromRoute] int noteId)
        {
            var note = await _service.GetNoteByNoteIdAsync(noteId);

            if(note is null)
                return NotFound();

            return Ok(note);
        }

        [HttpPut("{noteId:int}")]
        public async Task<IActionResult> UpdateNote([FromRoute] int noteId, [FromBody] NoteEdit updatedNote)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if(! await _service.UpdateNoteAsync(noteId, updatedNote))
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Ok();
        }







        // private NoteService CreateNoteService()
        // {
        //     var userId = int.Parse(_userManager.GetUserId(User));

        //     //var attempt = _userManager.GetUserAsync(HttpContext.User);

        //     var anotherAttempt = (ClaimsIdentity)this.User.Identity;
        //     var claim = anotherAttempt.FindFirst("Id");
        //     var hopefulId = int.Parse(claim.Value);

        //     // if(userId is null)
        //     //     throw new Exception("You must be signed in to access the Notes section");

        //     //return new NoteService(_options, userId);

        //     //return new NoteService(_options, attempt.Id);

        //     return new NoteService(_options, hopefulId);
        // }
    }
}