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
    public async Task<IActionResult> GetFriends()
    {
        try
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var friends = await GetFriendsList(true);
            
            return Ok(friends);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("/Friends/Requests")]
    public async Task<IActionResult> PendingFriendRequest(int id)
    {
        try
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var incomingRequests = await GetFriendsList(false);
            return Ok(new
            {
                IncomingRequests = incomingRequests.Select(x => x.Friends)
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [HttpPost("/Friends/Accept/{id:int:required}")]
    public async Task<IActionResult> AcceptFriend(int id)
    {
        
        return Ok();
    }

    private async Task<List<User>> GetFriendsList(bool accepted)
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        return await _hazeContext.Users.Include(x => x.Friends).ThenInclude(x => x.Friend)
            .ThenInclude(x => x.User).ThenInclude(x => x.FavouriteCategories).ThenInclude(x => x.Category)
            .Include(x => x.Friends).ThenInclude(x => x.Friend)
            .ThenInclude(x => x.User).ThenInclude(x => x.FavouritePlatforms).ThenInclude(x => x.Platform)
            .Include(x => x.Friends).ThenInclude(x => x.Friend)
            .ThenInclude(x => x.User).ThenInclude(x => x.WishList)
            .Select(x => new User()
            {
                Id = x.Id,
                Friends = x.Friends.Select(y => new UserFriend()
                {
                    Id = y.Id,
                    Friend = new Friend()
                    {
                        Id = y.Friend.Id,
                        Accepted = y.Friend.Accepted,
                        DateAccepted = y.Friend.DateAccepted,
                        DateAdded = y.Friend.DateAdded,
                        IsFamily = y.Friend.IsFamily,
                        User = new User()
                        {
                            Id = y.Friend.User.Id,
                            Username = y.Friend.User.Username,
                            FirstName = y.Friend.User.FirstName,
                            LastName = y.Friend.User.LastName,
                            FavouriteCategories = y.Friend.User.FavouriteCategories,
                            FavouritePlatforms = y.Friend.User.FavouritePlatforms,
                            WishList = y.Friend.User.WishList
                        }
                    }
                })
            })
            .Where(x => x.Id == userId && x.Friends.Any(t => t.Friend.Accepted == accepted)).ToListAsync();
    }
}