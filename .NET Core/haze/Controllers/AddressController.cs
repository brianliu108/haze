using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace haze.Controllers;

public class AddressController : Controller
{
    private HazeContext _hazeContext;
    public AddressController(IConfiguration configuration, HazeContext hazeContext)
    {
        _hazeContext = hazeContext;
    }
    
    [HttpGet("/Addresses")]
    [Authorize]
    public async Task<IActionResult> GetAddresses()
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        User user = await _hazeContext.Users.Include(x => x.BillingAddress)
            .Include(x => x.ShippingAddress).Where(x => x.Id == userId).FirstOrDefaultAsync();
        return Ok(new
        {
            ShippingAddress = user?.ShippingAddress,
            BillingAddress = user?.BillingAddress
        });
    }
    
    [HttpPut("/ShippingAddress")]
    [Authorize]
    public async Task<IActionResult> PutShippingAddress([FromBody] Address address)
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        User user = await _hazeContext.Users.Include(x => x.BillingAddress)
            .Include(x => x.ShippingAddress).Where(x => x.Id == userId).FirstOrDefaultAsync();
        user.ShippingAddress = address;
        await _hazeContext.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPut("/BillingAddress")]
    [Authorize]
    public async Task<IActionResult> PutBillingAddress([FromBody] Address address)
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        User user = await _hazeContext.Users.Include(x => x.BillingAddress)
            .Include(x => x.ShippingAddress).Where(x => x.Id == userId).FirstOrDefaultAsync();
        user.BillingAddress = address;
        await _hazeContext.SaveChangesAsync();
        return Ok();
    }
}