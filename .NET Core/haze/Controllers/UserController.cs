using haze.DataAccess;
using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace haze.Controllers
{
    [ApiController]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;

        private HazeContext _hazeContext;
        public UserController(ILogger<UserController> logger, HazeContext hazeContext)
        {
            _logger = logger;
            _hazeContext = hazeContext;
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform).ToListAsync());
        }

        [HttpGet("/GetUser/{id}")]
        public async Task<ActionResult<User>> GetUser()
        {
            var user = await _hazeContext.Users.FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("User not found!");
            return Ok(user);
        }

        [HttpPost("/CreateUser")]
        public async Task<IActionResult> Create(User request)
        {
            _hazeContext.Users.Add(request);
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("/testcreate")]
        public async Task<IActionResult> TestCreate(User request)
        {
            List<FavouritePlatform> favouritePlatforms = new List<FavouritePlatform>();

            Platform platform = await _hazeContext.Platforms.Where(x => x.Id == request.FavouritePlatforms[0].Platform.Id).FirstOrDefaultAsync();
            FavouritePlatform favouritePlatform = new FavouritePlatform()
            {
                Platform = platform
            };
            favouritePlatforms.Add(favouritePlatform);
            request.FavouritePlatforms = favouritePlatforms;
            _hazeContext.Users.Add(request);
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPatch("/UpdateUserPreferences")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdateUserPreferences preferences)
        {
            try
            {
                List<Category> categories = new List<Category>();
                List<Platform> platforms = new List<Platform>();
                if (preferences == null)
                    return BadRequest();
                var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                var user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform).Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (preferences?.CategoryIds != null || preferences.CategoryIds?.Count > 0)
                    categories = await _hazeContext.Categories.Where(x => preferences.CategoryIds.Contains(x.Id)).ToListAsync();

                if (preferences?.PlatformIds != null || preferences?.PlatformIds?.Count > 0)
                    platforms = await _hazeContext.Platforms.Where(x => preferences.PlatformIds.Contains(x.Id)).ToListAsync();
                
                // does this stay null if no categories given?
                if (categories != null)
                {                    
                    if (user.FavouriteCategories == null)
                    {
                        user.FavouriteCategories = new List<FavouriteCategory>();
                        for (int i = 0; i < categories.Count; i++)
                        {
                            user.FavouriteCategories.Add(new FavouriteCategory() { Category = categories[i] });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < categories.Count; i++)
                        {
                            if (user.FavouriteCategories.ElementAtOrDefault(i) != null)
                                user.FavouriteCategories[i].Category = categories[i];
                            else
                                user.FavouriteCategories.Add(new FavouriteCategory() { Category = categories[i] });
                        }
                        int userFavCatsCount = user.FavouriteCategories.Count;
                        for (int i = userFavCatsCount - categories.Count; i >= categories.Count; i--)
                        {
                            user.FavouriteCategories.RemoveAt(i);
                        }
                    }
                }
                if (platforms != null)
                {
                    if (user.FavouritePlatforms == null)
                    {
                        user.FavouritePlatforms = new List<FavouritePlatform>();
                        for (int i = 0; i < platforms.Count; i++)
                        {
                            user.FavouritePlatforms.Add(new FavouritePlatform() { Platform = platforms[i] });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < categories.Count; i++)
                        {
                            if (user.FavouritePlatforms.ElementAtOrDefault(i) != null)
                                user.FavouritePlatforms[i].Platform = platforms[i];
                            else
                                user.FavouritePlatforms.Add(new FavouritePlatform() { Platform = platforms[i] });
                        }
                        int userFavPlatsCount = user.FavouritePlatforms.Count;
                        for (int i = userFavPlatsCount - platforms.Count; i >= platforms.Count; i--)
                        {
                            user.FavouritePlatforms.RemoveAt(i);
                        }
                    }
                }
                await _hazeContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
                        
            return Ok();
        }

        [HttpPut("/UpdateUser")]
        public IActionResult Update([FromBody] User request)
        {
            return Ok();
        }

        [HttpDelete("/DeleteUser/{Id}")]
        public IActionResult Delete(int Id)
        {
            return Ok();
        }

        [HttpGet("/TestAuth")]
        [Authorize(Roles = "User")]
        public IActionResult TestAuthRoute()
        {
            var test = HttpContext.User;

            return Ok();
        }
    }
}