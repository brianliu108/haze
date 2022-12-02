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

    [HttpGet("/Reports/MemberFriends")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetMemberFriendsReport()
    {
        try
        {
            var users = await _hazeContext.Users.Where(x => x.RoleName == "User").ToListAsync();
            List<MemberFriendsReportJSON> memberFriends = new List<MemberFriendsReportJSON>();
            foreach (var user in users)
            {   
                var friends = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                    .Where(x => (x.User1.Id == user.Id || x.User2.Id == user.Id && x.Status == FriendStatus.Accepted))
                    .Select(x => new Friend()
                    {
                        Id = x.Id,
                        User1 = new User()
                        {
                            Id = x.User1.Id,
                            Username = x.User1.Username
                        },
                        User2 = new User()
                        {
                            Id = x.User2.Id,
                            Username = x.User2.Username
                        },
                        Status = x.Status,
                        DateAccepted = x.DateAccepted,
                        DateAdded = x.DateAdded,
                        User1IsFamily = x.User1IsFamily,
                        User2IsFamily = x.User2IsFamily
                    }).ToListAsync();
                memberFriends.Add(new MemberFriendsReportJSON()
                {
                    Username = user.Username,
                    NumberOfFriends = friends.Count
                });
            }
            return Ok(memberFriends);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("/Reports/ProductSales")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetProductSalesReport()
    {
        try
        {
            Dictionary<string, int> productSales = new Dictionary<string, int>();
            List<SalesReportJSON> salesReport = new List<SalesReportJSON>();
            var userProducts = await _hazeContext.UserProducts.Include(x => x.Product).ToListAsync();
            foreach (var userproduct in userProducts)
            {
                if (!productSales.ContainsKey(userproduct.Product.ProductName))
                    productSales.Add(userproduct.Product.ProductName, 0);
                productSales[userproduct.Product.ProductName] += 1;
            }
            foreach(KeyValuePair<string, int> productSale in productSales)
            {
                salesReport.Add(new SalesReportJSON()
                {
                    GameTitle = productSale.Key,
                    Sales = productSale.Value
                });   
            }
            return Ok(salesReport);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}