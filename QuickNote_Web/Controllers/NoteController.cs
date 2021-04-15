using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickNote_Models.Note;
using QuickNote_Services.Note;

namespace QuickNote_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _service;

        public NoteController(INoteService service)
        {
            _service = service;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateNote([FromBody] NoteCreate model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var wasCreated = await _service.CreateNoteAsync(model);

            if(wasCreated)
                return Ok("Your note was create");

            return BadRequest("Your note could not be created");
        }
    }
}