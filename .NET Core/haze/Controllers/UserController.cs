using haze.DataAccess;
using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace haze.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private HazeContext _hazeContext;
        public UserController(ILogger<UserController> logger, HazeContext hazeContext)
        {
            _logger = logger;
            _hazeContext = hazeContext;
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _hazeContext.Users.ToListAsync());
        }

        [HttpGet("/GetUser/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _hazeContext.Users.FindAsync(id);
            if (user == null)
                return BadRequest("User not found!");
            return Ok(user);
        }

        [HttpPost("/CreateUser")]
        public async Task<IActionResult> Create(User request)
        {
            _hazeContext.Users.Add(request);
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("/UpdateUser")]
        public async Task<IActionResult> ProfileUpdate(User request)
        {
            User user = await _hazeContext.Users.FindAsync(request.Id);
            if (user == null)
                return BadRequest("User not found!");

            var test = HttpContext.User;

            if (test.Identity.IsAuthenticated)
            {
                user.BirthDate = request.BirthDate;
                user.Username = request.Username;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.Password = request.Password;
                user.Gender = request.Gender;
                user.Newsletter = request.Newsletter;
                user.RoleName = request.RoleName;

                await _hazeContext.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest("User not Authenticated!");
            }
        }

        [HttpDelete("/DeleteUser/{Id}")]
        public IActionResult Delete(int Id)
        {
            return Ok();
        }

        [HttpGet("/TestAuth")]
        [Authorize(Roles ="Admin")]
        public IActionResult TestAuthRoute()
        {
            var test = HttpContext.User;
            return Ok();
        }
    }
}