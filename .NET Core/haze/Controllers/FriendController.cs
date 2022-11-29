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
            var friends = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => (x.User1.Id == userId || x.User2.Id == userId && x.Accepted))
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
                    Accepted = x.Accepted,
                    DateAccepted = x.DateAccepted,
                    DateAdded = x.DateAdded,
                    IsFamily = x.IsFamily
                }).ToListAsync();
            
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
            var pendingRequests = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
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
                    Accepted = x.Accepted,
                    DateAccepted = x.DateAccepted,
                    DateAdded = x.DateAdded,
                    IsFamily = x.IsFamily
                })
                .Where(x => x.User1.Id == userId && x.User2.Id != userId && !x.Accepted)
                .ToListAsync();
            var incomingRequests = await _hazeContext.Friends.Include(x => x.User1)
                .Include(x => x.User2).Where(x => x.User1.Id != userId && x.User2.Id == userId && !x.Accepted)
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
                    Accepted = x.Accepted,
                    DateAccepted = x.DateAccepted,
                    DateAdded = x.DateAdded,
                    IsFamily = x.IsFamily
                })
                .ToListAsync();
            return Ok(new
            {
                PendingRequests = pendingRequests,
                IncomingRequests = incomingRequests
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("/Friends/Requests/Send/{friendId:int:required}")]
    public async Task<IActionResult> SendFriendRequest(int friendId)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            var currentPendingRequests = await _hazeContext.Friends.Where(x => x.User1.Id == userId && x.User2.Id != userId && !x.Accepted)
                .ToListAsync();
            var friendCurrentPendingRequests = await _hazeContext.Friends.Where(x => x.User1.Id == friendId && x.User2.Id != friendId && !x.Accepted)
                .ToListAsync();
            // Validation
            if (friend == null)
                errors.Add("The given user wasn't found!");
            else if (friend.Id == userId)
                errors.Add("LMAO why u tryna add urself bro");
            else if (currentPendingRequests.Any(x => x.User1.Id == userId && x.User2.Id == friendId))
                errors.Add($"A friend request to user {friendId.ToString()} was already sent!");
            else if (friendCurrentPendingRequests.Any(x => x.User1.Id == friendId && x.User2.Id == userId))
                errors.Add($"You already have a friend request from user {friendId.ToString()}. You should try accepting their request instead");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
            _hazeContext.Friends.Add(new Friend()
            {
                Accepted = false,
                User1 = user,
                User2 = friend,
                DateAdded = DateTime.Today,
                IsFamily = false
            });
            
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("/Friends/Requests/Accept/{friendId:int:required}")]
    [Authorize]
    public async Task<IActionResult> AcceptFriend(int friendId)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            var currentPendingRequests = await _hazeContext.Friends.Where(x => x.User1.Id == userId && x.User2.Id != userId && !x.Accepted)
                .ToListAsync();
            var friendCurrentPendingRequests = await _hazeContext.Friends.Where(x => x.User1.Id == friendId && x.User2.Id != friendId && !x.Accepted)
                .ToListAsync();
            if (friend == null)
                errors.Add("The given user was not found!");
            else if (userId == friendId) 
                errors.Add("You can't accept a request from yourself, bro");
            else if (!friendCurrentPendingRequests.Any(x => x.User1.Id == friendId && x.User2.Id == userId))
                errors.Add("There isn't a pending request from the given user to accept!");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
            var friendRequest = friendCurrentPendingRequests
                .First(x => x.User1.Id == friendId && x.User2.Id == userId);
            friendRequest.Accepted = true;
            friendRequest.DateAccepted = DateTime.Today;
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    // private async Task<List<IEnumerable<UserFriend>?>> GetFriendsList(int userId, bool accepted)
    // {
    //     return await _hazeContext.Users.Include(x => x.Friends).ThenInclude(x => x.Friend)
    //         .ThenInclude(x => x.User).ThenInclude(x => x.FavouriteCategories).ThenInclude(x => x.Category)
    //         .Include(x => x.Friends).ThenInclude(x => x.Friend)
    //         .ThenInclude(x => x.User).ThenInclude(x => x.FavouritePlatforms).ThenInclude(x => x.Platform)
    //         .Include(x => x.Friends).ThenInclude(x => x.Friend)
    //         .ThenInclude(x => x.User).ThenInclude(x => x.WishList)
    //         .Select(x => new User()
    //         {
    //             Id = x.Id,
    //             Friends = x.Friends.Select(y => new UserFriend()
    //             {
    //                 Id = y.Id,
    //                 Friend = new Friend()
    //                 {
    //                     Id = y.Friend.Id,
    //                     Accepted = y.Friend.Accepted,
    //                     DateAccepted = y.Friend.DateAccepted,
    //                     DateAdded = y.Friend.DateAdded,
    //                     IsFamily = y.Friend.IsFamily,
    //                     User = new User()
    //                     {
    //                         Id = y.Friend.User.Id,
    //                         Username = y.Friend.User.Username,
    //                         FirstName = y.Friend.User.FirstName,
    //                         LastName = y.Friend.User.LastName,
    //                         FavouriteCategories = y.Friend.User.FavouriteCategories,
    //                         FavouritePlatforms = y.Friend.User.FavouritePlatforms,
    //                         WishList = y.Friend.User.WishList
    //                     }
    //                 }
    //             })
    //         })
    //         .Where(x => x.Id == userId && x.Friends.Any(t => t.Friend.Accepted == accepted)).Select(x => x.Friends).ToListAsync();
    // }
    //
    // private async Task<List<User>?> GetIncomingRequests()
    // {
    //     var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
    //     return await _hazeContext.Users.Include(x => x.Friends).ThenInclude(x => x.Friend)
    //         .ThenInclude(x => x.User).ThenInclude(x => x.FavouriteCategories).ThenInclude(x => x.Category)
    //         .Include(x => x.Friends).ThenInclude(x => x.Friend)
    //         .ThenInclude(x => x.User).ThenInclude(x => x.FavouritePlatforms).ThenInclude(x => x.Platform)
    //         .Include(x => x.Friends).ThenInclude(x => x.Friend)
    //         .ThenInclude(x => x.User).ThenInclude(x => x.WishList)
    //         .Where(x => x.Id != userId)
    //         .Where(x => x.Friends.Any(a => a.Friend.User.Id == userId))
    //         .Select(x => new User()
    //         {
    //             Id = x.Id,
    //             Username = x.Username,
    //             Friends = x.Friends.Select(y => new UserFriend()
    //             {
    //                 Id = y.Id,
    //                 Friend = new Friend()
    //                 {
    //                     Id = y.Friend.Id,
    //                     Accepted = y.Friend.Accepted,
    //                     DateAccepted = y.Friend.DateAccepted,
    //                     DateAdded = y.Friend.DateAdded,
    //                     IsFamily = y.Friend.IsFamily,
    //                     User = new User()
    //                     {
    //                         Id = y.Friend.User.Id,
    //                         Username = y.Friend.User.Username,
    //                         FirstName = y.Friend.User.FirstName,
    //                         LastName = y.Friend.User.LastName,
    //                         FavouriteCategories = y.Friend.User.FavouriteCategories,
    //                         FavouritePlatforms = y.Friend.User.FavouritePlatforms,
    //                         WishList = y.Friend.User.WishList
    //                     }
    //                 }
    //             })
    //         })
    //         .ToListAsync();
    // }
}