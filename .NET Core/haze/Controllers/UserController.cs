using haze.DataAccess;
using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace haze.Controllers
{
    [Route("/")]
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

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("FUCK");
        }

        [HttpGet("/GetUsers")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _hazeContext.Users.ToListAsync());
        }

        [HttpPost("/CreateUser")]
        public async Task<IActionResult> Create(User request)
        {
            //User user = new User();
            //user.Email = request.Email;
            //user.Username = request.Username;
            //user.Password = request.Password;
            //user.FirstName = request.FirstName;
            //user.LastName = request.LastName;
            //user.BirthDate = request.BirthDate;
            //user.Gender = request.Gender;
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
            return Ok();
        }
    }
}