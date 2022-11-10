using haze.DataAccess;
using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Net.NetworkInformation;

namespace haze.Controllers
{
    [ApiController]
    public class EventController : Controller
    {
        private readonly ILogger<EventController> _logger;

        private HazeContext _hazeContext;
        public EventController(ILogger<EventController> logger, HazeContext hazeContext)
        {
            _logger = logger;
            _hazeContext = hazeContext;
        }

        [HttpGet("GetEvents")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<Event>>> GetEvents()
        {
            return Ok(await _hazeContext.Events.Include(x => x.Products).ThenInclude(x => x.Categories)
                .Include(x => x.Products).ThenInclude(x => x.Platforms)
                .ToListAsync());
        }

        [HttpGet("/GetEvent/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Event>> GetEvent(int Id)
        {
            var e = await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Categories)
                .Include(x => x.Products).ThenInclude(x => x.Platforms)
                .Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (e == null)
                return BadRequest("Event not found!");

            //_hazeContext.Events.Remove(eventDelete);
            //await _hazeContext.SaveChangesAsync();

            return Ok(e);
        }

        [HttpPost("/AddEvent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEvent([FromBody] Event e)
        {
            _hazeContext.Events.Add(e);

            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("/DeleteEvent/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEvent(int Id)
        {
            var e = await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Categories)
                .Include(x => x.Products).ThenInclude(x => x.Platforms)
                .Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (e == null)
                return BadRequest("Event not found!");

            _hazeContext.Events.Remove(e);
            await _hazeContext.SaveChangesAsync();

            return Ok(e);
        }

        [HttpPut("/UpdateEvent")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent([FromBody] Event e)
        {
            Event eToUpdate= _hazeContext.Events.Where(p => p.Id == e.Id).First();

            eToUpdate = e;

            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

    }
}
