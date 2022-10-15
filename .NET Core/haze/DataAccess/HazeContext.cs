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
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Platform> Platforms { get; set; }
    }
}
