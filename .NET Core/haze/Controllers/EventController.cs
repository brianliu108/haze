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
        [Authorize]
        public async Task<ActionResult<List<Event>>> GetEvents()
        {
            return Ok(await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Product)
                .Include(x => x.RegisteredUsers).ThenInclude(x => x.RegisteredUser)
                .ToListAsync());
        }

        [HttpGet("RegisteredEvents")]
        [Authorize]
        public async Task<ActionResult<List<Event>>> GetUsersRegisteredEvents()
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            User user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category)
                .Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform)
                .Include(x => x.PaymentInfos).Where(x => x.Id == userId)
                .Include(x => x.BillingAddress).Include(x => x.ShippingAddress).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("User not found");

            List<Event> events = await _hazeContext.Events
                .Include(x => x.RegisteredUsers).ThenInclude(x => x.RegisteredUser)
                .Where(x=>x.RegisteredUsers.Count() > 0).ToListAsync();

            List<Event> registeredEvents = new List<Event>();

            foreach (var item in events)
            {
                var regUsers = item.RegisteredUsers.Where(u=>u.RegisteredUser.Id == userId).ToList();
                if (regUsers.Any())
                {
                    registeredEvents.Add(item);
                }
            }

            return Ok(registeredEvents);
        }

        [HttpGet("/Event/{Id}")]
        [Authorize]
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

            return Ok();
        }

        [HttpPut("/Event")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent([FromBody] EventJSON eJSON)
        {
            Event e = await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Product)
                .Include(x => x.RegisteredUsers).ThenInclude(x => x.RegisteredUser)
                .Where(x => x.Id == eJSON.Id).FirstOrDefaultAsync();

            if (e == null)
                return BadRequest("Еvent dont exist!");

            for (int i = 0; i < e.Products.Count; i++)
            {
                _hazeContext.EventProducts.Remove(e.Products[i]);
            }

            e.EventName = eJSON.EventName;
            e.StartDate = eJSON.StartDate;
            e.EndDate = eJSON.EndDate;
            e.Products = new List<EventProduct>();

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

        [HttpPost("/RegisterForEvent/{eventId}")]
        [Authorize]
        public async Task<IActionResult> RegisterForEvent(int eventId)
        {
            Event e = await _hazeContext.Events
                .Include(x => x.Products).ThenInclude(x => x.Product)
                .Include(x => x.RegisteredUsers).ThenInclude(x => x.RegisteredUser)
                .Where(x => x.Id == eventId).FirstOrDefaultAsync();

            if (e == null)
                return BadRequest("Event not found");

            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            User user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category)
                .Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform)
                .Include(x => x.PaymentInfos).Where(x => x.Id == userId)
                .Include(x => x.BillingAddress).Include(x => x.ShippingAddress).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("User not found");

            var userToRemove = e.RegisteredUsers.Where(e => e.RegisteredUser.Id == userId).FirstOrDefault();

            if (userToRemove == null)
            {
                e.RegisteredUsers.Add(new EventUser
                {
                    RegisteredUser = user
                });
            }
            else
            {
                e.RegisteredUsers.Remove(userToRemove);
            }


            await _hazeContext.SaveChangesAsync();

            return Ok();
        }
    }
}
