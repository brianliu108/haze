using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace haze.Controllers;

public class ReportController : Controller
{
    private HazeContext _hazeContext;
    public ReportController(HazeContext hazeContext)
    {
        _hazeContext = hazeContext;
    }
    
    [HttpGet("/Reports/Wishlist")]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> GetWishListReport()
    {
        List<WishlistReportJSON> reportList = new List<WishlistReportJSON>();
        List<WishlistItem> wishlistItems = await _hazeContext.WishlistItems.Include(x => x.Product)
            .ThenInclude(x => x.Categories).ThenInclude(x => x.сategory)
            .Include(x => x.Product).ThenInclude(x => x.Platforms).ThenInclude(x => x.platform).ToListAsync();
        var wishlistProducts = wishlistItems.GroupBy(x => x.Product).ToList();
        for (int i = 0; i < wishlistProducts.Count(); i++)
        {
            var wishlistProduct = wishlistProducts[i];
            reportList.Add(new WishlistReportJSON()
            {
                ProductName = wishlistProduct.Key.ProductName,
                NumberOfTimesWishlisted = wishlistProduct.Count()
            });
        }
        return Ok(reportList);
    }
}