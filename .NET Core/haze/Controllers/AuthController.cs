using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Security;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using BC = BCrypt.Net.BCrypt;
using haze.Controllers.Utility;
using System.Text.RegularExpressions;

namespace haze.Controllers
{
    public class AuthController : Controller
    {
        private IConfiguration _configuration;
        private HazeContext _hazeContext;
        public AuthController(IConfiguration configuration, HazeContext hazeContext)
        {
            _configuration = configuration;
            _hazeContext = hazeContext;
        }

        [HttpPost("/Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] User? user)
        {
            if (user == null || user.Username == null || user.Password == null)
                return BadRequest();

            User? queriedUser = null;            
            if (_hazeContext.Users != null)
                queriedUser = await _hazeContext.Users.Where(x => x.Username == user.Username).Where(x => x.Password == user.Password).FirstOrDefaultAsync();
            if (queriedUser == null)
                return NotFound();
            AuthUtility utility = new AuthUtility(_configuration);
            string jwt = utility.GenerateToken(queriedUser);
            string role = queriedUser.RoleName;
            return Ok(new
            {
                Token = jwt,
                Role = role
            });
        }

        [AllowAnonymous]
        [HttpPost("/Register")]
        public async Task<IActionResult> Register([FromBody] User? user)
        {            
            if (user == null || user.Username == null || user.Password == null)
                return BadRequest();
            List<string> errors = new List<string>();
            Regex passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            var usernameQuery = await _hazeContext.Users.Where(x => x.Username == user.Username).FirstOrDefaultAsync();
            var emailQuery = await _hazeContext.Users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();
            if (usernameQuery != null)
                errors.Add("The username is already taken!");
            if (emailQuery != null)
                errors.Add("The email is already in use!");
            if (!passwordRegex.IsMatch(user.Password))
                errors.Add("Password must be minimum eight characters, at least one letter and one number!");

            if (errors.Count > 0)
            {
                var response = new
                {
                    Errors = errors
                };
                if (errors.Contains("The username is already taken!") || errors.Contains("The email is already in use!"))
                    return Conflict(response);
                else return BadRequest(response);
            }
            user.RoleName = "User";
            user.Verified = false;
            user.Newsletter = true;
            _hazeContext.Users.Add(user);
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPost("/RegisterAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] User? user)
        {    
            List<string> errors = new List<string>();
            Regex passwordRegex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            if (user == null || user.FirstName == null || user.LastName == null || user.Password == null || user.BirthDate == null)
                errors.Add("A firstname, lastname, password and birth date is required!");
            if (!passwordRegex.IsMatch(user.Password))
                errors.Add("Password must be minimum eight characters, at least one letter and one number!");
            if (errors.Count > 0)
                return BadRequest(new
                {
                    Errors = errors
                });
            string adminDomain = @"cvgs.org";
            string fname = user.FirstName.ToLower();
            string lname = user.LastName.ToLower();
            string adminQueryEmail = $"{fname}.{lname}@{adminDomain}";
            
            var adminQuery = await _hazeContext.Users.Where(x => x.Email == adminQueryEmail).FirstOrDefaultAsync();
            int whileCounter = 1;
            while (adminQuery != null)
            {
                adminQueryEmail = $"{fname}.{lname}{whileCounter.ToString()}@{adminDomain}";
                adminQuery = await _hazeContext.Users.Where(x => x.Email == adminQueryEmail).FirstOrDefaultAsync();
                whileCounter++;
            }

            user.Email = adminQueryEmail;
            user.Username = adminQueryEmail;
            user.RoleName = "Admin";
            user.Verified = true;
            user.Newsletter = false;
            _hazeContext.Users.Add(user);
            await _hazeContext.SaveChangesAsync();
            return Ok(user);
        }
    }
}
