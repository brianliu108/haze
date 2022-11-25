using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace haze.Controllers;

public class FriendController : Controller
{
    private HazeContext _hazeContext;
    public FriendController(HazeContext hazeContext)
    {
        _hazeContext = hazeContext;
    }
    
    [HttpGet("/Friends")]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        
        try
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var friends = await _hazeContext.Users.Include(x => x.Friends)
                .Where(x => x.Id == userId).Select(x => new User()
            {
                Friends = x.Friends
            }).ToListAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}