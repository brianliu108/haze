using haze.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Transactions;

namespace haze.DataAccess
{
    public class UsersContext : DbContext
    {
        public UsersContext(DbContextOptions<UsersContext> options) : base(options) { }

        public DbSet<User>? Users { get; set; }
        public DbSet<PaymentInfo>? PaymentInfo { get; set; }
    }
}
