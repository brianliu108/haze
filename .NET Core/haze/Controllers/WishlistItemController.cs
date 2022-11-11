using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace haze.Controllers;

public class WishlistItemController : Controller
{
    private HazeContext _hazeContext;
    public WishlistItemController(HazeContext hazeContext)
    {
        _hazeContext = hazeContext;
    }
    
    [HttpGet("/WishList")]
    [Authorize]
    public async Task<IActionResult> GetWishListItems()
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        User user = await _hazeContext.Users.Include(x => x.WishList)
            .ThenInclude(x => x.Product).Where(x => x.Id == userId).FirstOrDefaultAsync();
        var wishlist = user.WishList;
        return Ok(wishlist);
    }
    
    [HttpPost("/WishList")]
    [Authorize]
    public async Task<IActionResult> PostWishListItems([FromBody] ProductIdJSON productIdJson)
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        User user = await _hazeContext.Users.Include(x => x.WishList)
            .ThenInclude(x => x.Product).Where(x => x.Id == userId).FirstOrDefaultAsync();
        var wishlist = user.WishList;
        if (wishlist == null)
            wishlist = new List<WishlistItem>();

        Product product = await _hazeContext.Products.Where(x => x.Id == productIdJson.ProductId).FirstOrDefaultAsync();
        if (product == null)
            return NotFound("Product not found");
        wishlist.Add(new WishlistItem()
        {
            Product = product
        });

        await _hazeContext.SaveChangesAsync();
        return Ok(wishlist);
    }

    [HttpDelete("/WishList/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteWishListItem(int id)
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        User user = await _hazeContext.Users.Include(x => x.WishList)
            .ThenInclude(x => x.Product).Where(x => x.Id == userId).FirstOrDefaultAsync();
        WishlistItem wishlistItem = user.WishList.Where(x => x.Id == id).FirstOrDefault();
        if (wishlistItem == null)
            return NotFound("Wishlist item not found");
        _hazeContext.WishlistItems.Remove(wishlistItem);

        await _hazeContext.SaveChangesAsync();
        return Ok();
    }
}