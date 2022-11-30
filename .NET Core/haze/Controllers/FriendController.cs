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
                .Where(x => (x.User1.Id == userId || x.User2.Id == userId && x.Status == FriendStatus.Accepted))
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
                    Status = x.Status,
                    DateAccepted = x.DateAccepted,
                    DateAdded = x.DateAdded,
                    User1IsFamily = x.User1IsFamily,
                    User2IsFamily = x.User2IsFamily
                })
                .Where(x => x.User1.Id == userId && x.User2.Id != userId && x.Status == FriendStatus.Pending)
                .ToListAsync();
            var incomingRequests = await _hazeContext.Friends.Include(x => x.User1)
                .Include(x => x.User2).Where(x => x.User1.Id != userId && x.User2.Id == userId && x.Status == FriendStatus.Pending)
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
            var currentPendingRequests = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => x.User1.Id == userId && x.User2.Id != userId && x.Status == FriendStatus.Pending)
                .ToListAsync();
            var friendCurrentPendingRequests = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => x.User1.Id == friendId && x.User2.Id != friendId && x.Status == FriendStatus.Pending)
                .ToListAsync();
            // Validation
            if (friend == null)
                errors.Add("The given user wasn't found!");
            else if (friend.Id == userId)
                errors.Add("LMAO why u tryna add urself bro");
            else if (currentPendingRequests.Where(x => x.User1.Id == userId && x.User2.Id == friendId).FirstOrDefault() != null)
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
                Status = FriendStatus.Pending,
                User1 = user,
                User2 = friend,
                DateAdded = DateTime.Today,
                User1IsFamily = false,
                User2IsFamily = false
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
            var currentPendingRequests = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => x.User1.Id == userId && x.User2.Id != userId && x.Status == FriendStatus.Pending)
                .ToListAsync();
            var friendCurrentPendingRequests = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => x.User1.Id == friendId && x.User2.Id != friendId && x.Status == FriendStatus.Pending)
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
            friendRequest.Status = FriendStatus.Accepted;
            friendRequest.DateAccepted = DateTime.Today;
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("/Friends/Requests/Ignore/{friendId:int:required}")]
    [Authorize]
    public async Task<IActionResult> IgnoreFriend(int friendId)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            var currentPendingRequests = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => x.User1.Id == userId && x.User2.Id != userId && x.Status == FriendStatus.Pending)
                .ToListAsync();
            var friendCurrentPendingRequests = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => x.User1.Id == friendId && x.User2.Id != friendId && x.Status == FriendStatus.Pending)
                .ToListAsync();
            if (friend == null)
                errors.Add("The given user was not found!");
            else if (userId == friendId) 
                errors.Add("You can't ignore a request from yourself, bro");
            else if (!friendCurrentPendingRequests.Any(x => x.User1.Id == friendId && x.User2.Id == userId))
                errors.Add("There isn't a pending request from the given user to ignore!");
            else if(await _hazeContext.Friends.Where(x => x.User1.Id == friendId && x.User2.Id == userId && x.Status == FriendStatus.Accepted).FirstOrDefaultAsync() != null)
                errors.Add("You can't ignore a friend that's already been accepted!");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
            var friendRequest = friendCurrentPendingRequests
                .First(x => x.User1.Id == friendId && x.User2.Id == userId);
            friendRequest.Status = FriendStatus.Ignored;

            await _hazeContext.SaveChangesAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("/Friends/Requests/Delete/{friendId:int:required}")]
    [Authorize]
    public async Task<IActionResult> DeleteFriend(int friendId)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            var currentFriendObject = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => (x.User1.Id == userId && x.User2.Id == friendId) ||
                            x.User1.Id == friendId && x.User2.Id == userId)
                .FirstOrDefaultAsync();
            if (friend == null)
                errors.Add("The given user was not found!");
            else if (userId == friendId) 
                errors.Add("You can't delete a request from yourself, bro");
            else if (currentFriendObject == null)
                errors.Add("There isn't a friend to delete here!");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
            _hazeContext.Friends.Remove(currentFriendObject);

            await _hazeContext.SaveChangesAsync();

            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("/Friends/Family/{friendId:int:required}")]
    [Authorize]
    public async Task<IActionResult> AddFamily(int friendId)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            var currentFriendObject = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => (x.User1.Id == userId && x.User2.Id == friendId) ||
                            x.User1.Id == friendId && x.User2.Id == userId)
                .FirstOrDefaultAsync();
            if (friend == null)
                errors.Add("The given user was not found!");
            else if (userId == friendId) 
                errors.Add("You can't add yourself as family!");
            else if (currentFriendObject == null)
                errors.Add("There isn't a friend to add as family here!");
            else if (currentFriendObject.Status != FriendStatus.Accepted)
                errors.Add("The other user hasn't accepted your request!");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
            if (userId == currentFriendObject.User1.Id)
                currentFriendObject.User1IsFamily = true;
            else
                currentFriendObject.User2IsFamily = true;

            await _hazeContext.SaveChangesAsync();
            
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpDelete("/Friends/Family/{friendId:int:required}")]
    [Authorize]
    public async Task<IActionResult> RemoveFamily(int friendId)
    {
        try
        {
            List<string> errors = new List<string>();
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            var friend = await _hazeContext.Users.Where(x => x.Id == friendId).FirstOrDefaultAsync();
            var currentFriendObject = await _hazeContext.Friends.Include(x => x.User1).Include(x => x.User2)
                .Where(x => (x.User1.Id == userId && x.User2.Id == friendId) ||
                            x.User1.Id == friendId && x.User2.Id == userId)
                .FirstOrDefaultAsync();
            if (friend == null)
                errors.Add("The given user was not found!");
            else if (userId == friendId) 
                errors.Add("You can't delete yourself as family!");
            else if (currentFriendObject == null)
                errors.Add("There isn't a friend to add as family here!");
            else if (currentFriendObject.Status != FriendStatus.Accepted)
                errors.Add("The other user hasn't accepted your request!");
            else if ((currentFriendObject.User1.Id == userId && !currentFriendObject.User1IsFamily) || (currentFriendObject.User2.Id == userId && !currentFriendObject.User2IsFamily))
                errors.Add("You haven't set this friend to family yet!");
            if (errors.Count != 0)
                return NotFound(new
                {
                    Errors = errors
                });
            if (userId == currentFriendObject.User1.Id)
                currentFriendObject.User1IsFamily = false;
            else
                currentFriendObject.User2IsFamily = false;

            await _hazeContext.SaveChangesAsync();
            
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}