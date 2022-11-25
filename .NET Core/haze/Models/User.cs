using Microsoft.AspNetCore.Identity;

namespace haze.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool Verified { get; set; }
        public bool Newsletter { get; set; }
        public string RoleName { get; set; }
        public List<PaymentInfo>? PaymentInfos { get; set; }
        public List<FavouritePlatform>? FavouritePlatforms { get; set; }
        public List<FavouriteCategory>? FavouriteCategories { get; set; }
        public Address? ShippingAddress { get; set; }
        public Address? BillingAddress { get; set; }
        public List<WishlistItem>? WishList { get; set; }
        public List<UserFriend>? Friends { get; set; }
    }
}
