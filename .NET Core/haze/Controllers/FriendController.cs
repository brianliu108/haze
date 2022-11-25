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
    [HttpPost("/Friends/Requests/Send/{id:int:required}")]
    public async Task<IActionResult> SendFriendRequest(int id)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Include(x => x.Friends).ThenInclude(x => x.Friend)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            var currentPendingRequests = await GetFriendsList(false);
            var currentPendingRequestsUsers = new List<User>();
            // foreach (var item in currentPendingRequests)
                if (friend == null)
            {
                errors.Add("The given user wasn't found!");
                return NotFound(errors);
            }
            // if (currentPendingRequests.Where(x => x.Friends != null && x.Friends.Where(y => y.Friend.User == friend)).ToList().Count != 0)
            // if (currentPendingRequests.Any(x => x.Friends != null && x.Friends.Any(y => y.Friend.User == friend)))
            var test = currentPendingRequests.Select(x => x.Friends).ToList();
            var anus = test.Where(x => x.)
                Console.WriteLine("gae");
            // if (currentPendingRequests.Any(x => x.Friends != null && x.Friends.Any(y => y.Friend.User == friend)))
            // {
            //     errors.Add("You've already sent a request to this user!");
            //     return BadRequest(errors);
            // }
            var friendsList = user.Friends.ToList();
            friendsList.Add(new UserFriend()
            {
                Friend = new Friend()
                {
                    Accepted = false,
                    DateAdded = DateTime.Today,
                    User = friend,
                    IsFamily = false
                }
            });
            user.Friends = friendsList;
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("/Friends/Requests")]
    [Authorize]
    public async Task<IActionResult> PendingFriendRequest(int id)
    {
        try
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var pendingRequests = await GetFriendsList(false);
            // var incomingRequests = await _hazeContext.Friends.Include(x => x.User)
            return Ok(new
            {
                pendingRequests = pendingRequests.Select(x => x.Friends)
                // incomingRequests = incomingRequests
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // [HttpGet("/test1")]
    // [Authorize]
    // public async Task<IActionResult> Test()
    // {
    //     
    // }

    [HttpPost("/Friends/Requests/Accept/{id:int:required}")]
    [Authorize]
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