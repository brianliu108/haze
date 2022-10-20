using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace haze.Controllers;

[ApiController]
public class CategoryController : Controller
{
    private HazeContext _hazeContext;
    public CategoryController(HazeContext hazeContext)
    {
        _hazeContext = hazeContext;
    }
    
    [HttpGet("Categories")]
    [Authorize]
    public async Task<ActionResult<List<Category>>> GetCategories()
    {
        try
        {
            return Ok(await _hazeContext.Categories.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest("Something went wrong finding Categories!");
        }   
    }

    [HttpGet("UserCategories")]
    [Authorize(Roles="User")]
    public async Task<ActionResult> GetUserCategories()
    {
        try
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            User user =  await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Where(x => x.Id == userId).FirstOrDefaultAsync();
            return Ok(new
            {
                Categories = user.FavouriteCategories
            });
        }
        catch (Exception e)
        {
            return BadRequest();
        }
    }
}