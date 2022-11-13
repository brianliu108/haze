using haze.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Transactions;

namespace haze.DataAccess
{
    public class HazeContext : DbContext
    {
        public HazeContext(DbContextOptions<HazeContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<PaymentInfo> PaymentInfo { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<FavouriteCategory> FavouriteCategories { get; set; }
        public DbSet<FavouritePlatform> FavouritePlatforms { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPlatform> ProductPlatforms { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventProduct> EventProducts { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<ProductUserReview> ProductUserReviews { get; set; }
    }
}
