using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace haze.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("/Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] User user)
        {
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var secret = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(secret),
            SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return Ok(stringToken);
        }

        [AllowAnonymous]
        [HttpPost("/Register")]
        public IActionResult Register([FromBody] User user)
        {
            return Ok();
        }
    }
}
