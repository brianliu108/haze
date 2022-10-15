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
        public async Task<ActionResult<User>> GetUser()
        {
            var user = await _hazeContext.Users.FirstOrDefaultAsync();
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
        public IActionResult Update([FromBody] User request)
        {
            return Ok();
        }

        [HttpDelete("/DeleteUser/{Id}")]
        public IActionResult Delete(int Id)
        {
            return Ok();
        }

        [HttpGet("/TestAuth")]
        [Authorize(Roles ="User")]
        public IActionResult TestAuthRoute()
        {
            var test = HttpContext.User;
            return Ok();
        }
    }
}