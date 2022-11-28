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
            var friends = await GetFriendsList(userId, true);
            
            return Ok(friends);
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
            var pendingRequests = await GetFriendsList(userId, false);
            return Ok(new
            {
                PendingRequests = pendingRequests,
                IncomingRequests = await GetIncomingRequests()
            });
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
                .ThenInclude(x => x.User).Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            var currentPendingRequests = await GetFriendsList(userId, false);
            if (friend == null)
                errors.Add("The given user wasn't found!");
            else if (friend.Id == userId)
                errors.Add("LMAO why u tryna add urself bro");
            else if (currentPendingRequests.Any(a => a.ToList().Any(x => x.Friend.User.Id == friend.Id)))
                errors.Add($"A friend request to user {friend.Username} was already sent!");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
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

    [HttpPost("/Friends/Requests/Accept/{id:int:required}")]
    [Authorize]
    public async Task<IActionResult> AcceptFriend(int id)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Include(x => x.Friends).ThenInclude(x => x.Friend)
                .ThenInclude(x => x.User).Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            var targetUser = await _hazeContext.Users.Include(x => x.Friends).ThenInclude(x => x.Friend)
                .ThenInclude(x => x.User).Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            var targetUserPendingRequests = await GetFriendsList(id, false);
            if (targetUser == null)
                errors.Add("The given user was not found!");
            if (errors.Count == 0 && !targetUserPendingRequests.Any(a => a.ToList().Any(x => x.Friend.User.Id == userId)))
                errors.Add("There isn't a pending request from the given user to accept!");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
            targetUser.Friends.Where(x => x.Friend.User.Id == userId).First().Friend.Accepted = true;

            await _hazeContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task<List<IEnumerable<UserFriend>?>> GetFriendsList(int userId, bool accepted)
    {
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
            .Where(x => x.Id == userId && x.Friends.Any(t => t.Friend.Accepted == accepted)).Select(x => x.Friends).ToListAsync();
    }

    private async Task<List<User>?> GetIncomingRequests()
    {
        var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
        return await _hazeContext.Users.Include(x => x.Friends).ThenInclude(x => x.Friend)
            .ThenInclude(x => x.User).ThenInclude(x => x.FavouriteCategories).ThenInclude(x => x.Category)
            .Include(x => x.Friends).ThenInclude(x => x.Friend)
            .ThenInclude(x => x.User).ThenInclude(x => x.FavouritePlatforms).ThenInclude(x => x.Platform)
            .Include(x => x.Friends).ThenInclude(x => x.Friend)
            .ThenInclude(x => x.User).ThenInclude(x => x.WishList)
            .Where(x => x.Id != userId)
            .Where(x => x.Friends.Any(a => a.Friend.User.Id == userId))
            .Select(x => new User()
            {
                Id = x.Id,
                Username = x.Username,
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
            .ToListAsync();
    }
}