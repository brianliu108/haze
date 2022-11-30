using haze.DataAccess;
using haze.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

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
            return Ok(await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category)
                .Include(x => x.FavouritePlatforms)
                .ThenInclude(x => x.Platform)
                .Include(x => x.PaymentInfos).Include(x => x.BillingAddress).Include(x => x.ShippingAddress)
                .ToListAsync());
        }


        [HttpGet("/GetUser")]
        [Authorize]
        public async Task<ActionResult<User>> GetUser()
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            var user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category)
                .Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform)
                .Include(x => x.PaymentInfos).Where(x => x.Id == userId)
                .Include(x => x.BillingAddress).Include(x => x.ShippingAddress).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("User not found!");
            return Ok(user);
        }

        [HttpPost("/CreateUser")]
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create(User request)
        {
            _hazeContext.Users.Add(request);
            await _hazeContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("/UserPreferences")]
        [Authorize]
        public async Task<IActionResult> GetUserPreferences()
        {
            try
            {
                var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                var user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform).Where(x => x.Id == userId).FirstOrDefaultAsync();
                UserPreferencesJSON preferencesJson = new UserPreferencesJSON();
                if (user.FavouritePlatforms != null && user.FavouritePlatforms.Count > 0)
                {
                    foreach (var platform in user.FavouritePlatforms)
                    {
                        preferencesJson.PlatformIds.Add(platform.Platform.Id);
                    }
                }
                if (user.FavouriteCategories != null && user.FavouriteCategories.Count > 0)
                {
                    foreach (var category in user.FavouriteCategories)
                    {
                        preferencesJson.CategoryIds.Add(category.Category.Id);
                    }
                }

                return Ok(preferencesJson);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPatch("/UserPreferences")]
        [Authorize]
        public async Task<IActionResult> UpdateUserPreferences([FromBody] UserPreferencesJSON preferencesJson)
        {
            try
            {
                List<Category> categories = new List<Category>();
                List<Platform> platforms = new List<Platform>();
                List<string> errors = new List<string>();
                if (preferencesJson == null)
                    return BadRequest();
                var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                var user = await _hazeContext.Users.Include(x => x.FavouriteCategories).ThenInclude(x => x.Category).Include(x => x.FavouritePlatforms).ThenInclude(x => x.Platform).Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return BadRequest();
                if (preferencesJson?.CategoryIds != null || preferencesJson.CategoryIds?.Count > 0)
                    categories = await _hazeContext.Categories.Where(x => preferencesJson.CategoryIds.Contains(x.Id)).ToListAsync();

                if (preferencesJson?.PlatformIds != null || preferencesJson?.PlatformIds?.Count > 0)
                    platforms = await _hazeContext.Platforms.Where(x => preferencesJson.PlatformIds.Contains(x.Id)).ToListAsync();

                if (categories.Count < preferencesJson.CategoryIds.Count)
                    errors.Add("One or more provided Categories were not found!");
                else if (categories.Count > 3)
                    errors.Add("Cannot add more than 3 favourite categories!");
                if (platforms.Count < preferencesJson.PlatformIds.Count)
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
                } else if (user.FavouriteCategories?.Count > 0 && preferencesJson.CategoryIds.Count == 0)
                {
                    user.FavouriteCategories.RemoveAll(x => x != null);
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
                        for (int i = 0; i < platforms.Count; i++)
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
                } else if (user.FavouritePlatforms?.Count > 0 && preferencesJson.PlatformIds.Count == 0)
                {
                    user.FavouritePlatforms.RemoveAll(x => x != null);
                }
                
                await _hazeContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }
        
        [HttpGet("/UserProfile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                User user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return BadRequest("User not found!");

                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }            
        }
        
        [HttpPut("/UserProfile")]
        [Authorize]
        public async Task<IActionResult> ProfileUpdate(User request)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
                User user = await _hazeContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return BadRequest("User not found!");

                user.BirthDate = request.BirthDate;
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Gender = request.Gender;
                user.Newsletter = request.Newsletter;

                await _hazeContext.SaveChangesAsync();

                return Ok();
            }
            catch
            {
                return BadRequest();
            }            
        }

        [HttpGet("/PaymentInfo")]
        [Authorize]
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

        [HttpPost("/PaymentInfo")]
        [Authorize]
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

        [HttpPut("/PaymentInfo")]
        [Authorize]
        public async Task<IActionResult> PaymentInfoUpdate([FromBody] PaymentInfo paymentInfo)
        {
            var userId = int.Parse(HttpContext.User.Claims.Where(x => x.Type == "userId").FirstOrDefault().Value);
            
            try
            {
                User user = await _hazeContext.Users.Include(x => x.PaymentInfos).Where(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return NotFound("User not found!");
                PaymentInfo userPaymentInfo = user.PaymentInfos.Where(p => p.Id == paymentInfo.Id).FirstOrDefault();
                if (userPaymentInfo == null)
                    return NotFound("The given payment info wasn't found!");
                userPaymentInfo.CreditCardNumber = paymentInfo.CreditCardNumber;
                userPaymentInfo.ExpiryDate = paymentInfo.ExpiryDate;
                await _hazeContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest("Error " + e.Message);
            }

            return Ok();
        }

        [HttpDelete("/PaymentInfo")]
        [Authorize]
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
            SmtpClient smtpClient = new SmtpClient("haze.brianliu.ca", 587);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("mail@haze.brianliu.ca", "Initial1");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;
            MailMessage mail = new MailMessage();

            //Setting From , To and CC
            mail.From = new MailAddress("mail@haze.brianliu.ca", "Test");
            mail.To.Add(new MailAddress("bliu3847@conestogac.on.ca"));
        
            ServicePointManager.ServerCertificateValidationCallback = 
                (sender, certificate, chain, sslPolicyErrors) => true;
        
            smtpClient.Send(mail);

            return Ok();
        }

        // [HttpGet("/Test")]
        // [Authorize]
        // public async Task<IActionResult> TestFriend()
        // {
        //
        //     User user = await _hazeContext.Users.Include(x => x.Friends).Where(x => x.Id == 2).FirstOrDefaultAsync();
        //     User friend = await _hazeContext.Users.Include(x => x.Friends).Where(x => x.Id == 3).FirstOrDefaultAsync();
        //     
        //     user.Friends.ToList().Add(new UserFriend()
        //     {
        //         Friend = new Friend()
        //         {
        //             Accepted = false,
        //             DateAdded = DateTime.Today,
        //             IsFamily = false,
        //             User = friend
        //         }
        //     });
        //
        //     // await _hazeContext.SaveChangesAsync();
        //     return Ok();
        // }
    }
}