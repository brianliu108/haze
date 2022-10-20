using haze.Models;
using Microsoft.AspNetCore.Mvc;

namespace haze.Controllers;

[ApiController]
public class PlatformController : Controller
{
    private HazeContext _hazeContext;
    
    public PlatformController(HazeContext hazeContext)
    {
        _hazeContext = hazeContext;
    }
    
    [HttpGet("Platforms")]
    public async Task<ActionResult<List<Platform>>> Index()
    {
        try
        {
            return Ok(await _hazeContext.Platforms.ToListAsync());
        }
        catch (Exception e)
        {
            return BadRequest("Something went wrong finding Platforms!");
        }
    }
}