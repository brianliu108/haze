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

        [HttpGet("Events")]
        public async Task<ActionResult<List<Event>>> GetEvents()
        {
            return Ok(await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Product)
                .Include(x => x.RegisteredUsers).ThenInclude(x => x.RegisteredUser)
                .ToListAsync());
        }

        [HttpGet("/Event/{Id}")]
        public async Task<ActionResult<Event>> GetEvent(int Id)
        {
            var e = await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Product)
                .Include(x => x.RegisteredUsers).ThenInclude(x => x.RegisteredUser)
                .Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (e == null)
                return BadRequest("Event not found!");

            return Ok(e);
        }

        [HttpPost("/Event")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEvent([FromBody] EventJSON eJSON)
        {
            Event e = new Event
            {
                Id = 123213,
                EventName = eJSON.EventName,
                StartDate = eJSON.StartDate,
                EndDate = eJSON.EndDate,
                Products = new List<EventProduct>(),
                RegisteredUsers = new List<EventUser>(),
            };

            _hazeContext.Events.Add(e);

            // Check if products options exist
            for (int i = 0; i < eJSON.ProductIds.Count; i++)
            {
                var eProd = await _hazeContext.Products.Where(x => x.Id == eJSON.ProductIds[i]).FirstOrDefaultAsync();

                if (eProd == null)
                    return BadRequest("Product not found!");

                e.Products.Add(new EventProduct
                {
                    Product = eProd
                });
            }


            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("/Event/{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteEvent(int Id)
        {
            var e = await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Product)
                .Include(x => x.RegisteredUsers).ThenInclude(x => x.RegisteredUser)
                .Where(x => x.Id == Id).FirstOrDefaultAsync();

            if (e == null)
                return BadRequest("Event not found!");

            _hazeContext.Events.Remove(e);
            await _hazeContext.SaveChangesAsync();

            return Ok(e);
        }

        [HttpPut("/Event")]
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
