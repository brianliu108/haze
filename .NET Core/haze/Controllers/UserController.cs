﻿using haze.DataAccess;
using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform).ToListAsync());
        }

        [HttpGet("/GetUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUser()
        {
            var user = await _hazeContext.Users.FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("User not found!");
            return Ok(user);
        }

        [HttpPost("/CreateUser")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create(User request)
        {
            _hazeContext.Users.Add(request);
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("/UserPreferences")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserPreferences()
        {
            try
            {
                var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                var user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform).Where(x => x.Id == userId).FirstOrDefaultAsync();
                UpdateUserPreferences preferences = new UpdateUserPreferences();
                if (user.FavouritePlatforms != null && user.FavouritePlatforms.Count > 0)
                {
                    foreach (var platform in user.FavouritePlatforms)
                    {
                        preferences.PlatformIds.Add(platform.Id);
                    }
                }
                if (user.FavouriteCategories != null && user.FavouriteCategories.Count > 0)
                {
                    foreach (var category in user.FavouriteCategories)
                    {
                        preferences.CategoryIds.Add(category.Id);
                    }
                }

                return Ok(preferences);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPatch("/UserPreferences")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateUserPreferences([FromBody] UpdateUserPreferences preferences)
        {
            try
            {
                List<Category> categories = new List<Category>();
                List<Platform> platforms = new List<Platform>();
                List<string> errors = new List<string>();
                if (preferences == null)
                    return BadRequest();
                var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                var user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform).Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (preferences?.CategoryIds != null || preferences.CategoryIds?.Count > 0)
                    categories = await _hazeContext.Categories.Where(x => preferences.CategoryIds.Contains(x.Id)).ToListAsync();

                if (preferences?.PlatformIds != null || preferences?.PlatformIds?.Count > 0)
                    platforms = await _hazeContext.Platforms.Where(x => preferences.PlatformIds.Contains(x.Id)).ToListAsync();

                if (categories.Count < preferences.CategoryIds.Count)
                    errors.Add("One or more provided Categories were not found!");
                else if (categories.Count > 3)
                    errors.Add("Cannot add more than 3 favourite categories!");
                if (platforms.Count < preferences.PlatformIds.Count)
                    errors.Add("One or more provided Platforms were not found!");
                else if (platforms.Count > 3)
                    errors.Add("Cannot add more than 3 favourite platforms!");
                if (errors.Count > 0)
                    return NotFound(new
                    {
                        Errors = errors
                    });

                if (categories.Count > 0)
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
                if (platforms.Count > 0)
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
        public async Task<IActionResult> ProfileUpdate(User request)
        {
            User user = await _hazeContext.Users.FindAsync(request.Id);
            if (user == null)
                return BadRequest("User not found!");

            var test = HttpContext.User;

            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);


            if (test == null || userId != request.Id)
            {
                return BadRequest("User not Authenticated!");
            }
            else
            {
                user.BirthDate = request.BirthDate;
                user.Username = request.Username;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.Password = request.Password;
                user.Gender = request.Gender;
                user.Newsletter = request.Newsletter;
                user.RoleName = request.RoleName;
            }


            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("/GetPaymentInfo")]
        public async Task<ActionResult<List<PaymentInfo>>> GetPaymentInfo()
        {
            int userIdHTTP = 0;
            try
            {
                if (HttpContext.User != null && HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault() != null)
                {
                    userIdHTTP = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                }
                if (userIdHTTP == 0)
                {
                    return BadRequest("User not authorized!");
                }
            }
            catch (Exception e)
            {

                return BadRequest("Error " + e);
            }

            User user = await _hazeContext.Users.Include(x => x.PaymentInfos).Where(x => x.Id == userIdHTTP).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("User not found");
            }
            else
            {
                return Ok(user.PaymentInfos);
            }
        }

        [HttpPost("/AddUserPaymentInfo")]
        public async Task<IActionResult> AddPaymentInfo([FromBody] PaymentInfo paymentInfo)
        {
            Regex expiryRegex = new Regex(@"^[\d][\d][\/][\d][\d]$");
            Regex cardRegex = new Regex(@"^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$");

            if (!Regex.IsMatch(paymentInfo.CreditCardNumber.ToString().ToLower(), @"^(?:4[0-9]{12}(?:[0-9]{3})?|[25][1-7][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$"))
            {
                return BadRequest("Credit card number is incorrect!");
            }


            int userId = 0;
            if (HttpContext.User != null && HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault() != null)
            {
                userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            }
            if (userId == 0)
            {
                return BadRequest("User not found!");
            }
            
            var user = await _hazeContext.Users.Include(x => x.PaymentInfos).Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("User not found!");
            }
            else
            {
                if (userId != user.Id)
                {
                    return BadRequest("User not found!");
                }
                user.PaymentInfos.Add(paymentInfo);
            }

            await _hazeContext.SaveChangesAsync();
            
            return Ok();
        }

        [HttpPut("/UpdatePaymentInfo")]
        public async Task<IActionResult> PaymentInfoUpdate([FromBody] PaymentInfo paymentInfo)
        {
            int userId = 0;
            try
            {
                if (HttpContext.User != null && HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault() != null)
                {
                    userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                }
                if (userId == 0)
                {
                    return BadRequest("User not found!");
                }
            }
            catch (Exception e)
            {
                return BadRequest("Error " + e);

            }

            User user = await _hazeContext.Users.Include(x => x.PaymentInfos).Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("User not found!");

            if (user.PaymentInfos.Count() == 0)
            {
                return BadRequest("Payment Info not found!");
            }

            PaymentInfo pInfo = user.PaymentInfos.Where(p => p.Id == paymentInfo.Id).First();

            pInfo = paymentInfo;


            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("/DeletePaymentInfo")]
        public async Task<IActionResult> DeletePaymentInfo([FromBody] PaymentInfo paymentInfo)
        {
            int userId = 0;
            try
            {
                if (HttpContext.User != null && HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault() != null)
                {
                    userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                }
                if (userId == 0)
                {
                    return BadRequest("User not found!");
                }
            }
            catch (Exception e)
            {
                return BadRequest("Error " + e);

            }

            User user = await _hazeContext.Users.Include(x => x.PaymentInfos).Where(x => x.Id == userId).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("User not found!");

            if (user.PaymentInfos.Count() == 0)
                return BadRequest("Payment Info not found!");

            PaymentInfo pInfo = user.PaymentInfos.Where(p => p.Id == paymentInfo.Id).First();

            _hazeContext.PaymentInfo.Remove(pInfo);
            await _hazeContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("/DeleteUser/{Id}")]
        public IActionResult DeleteUser(int Id)
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