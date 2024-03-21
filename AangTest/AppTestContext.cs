using Aang.Model;
using Microsoft.EntityFrameworkCore;

namespace AangTest
{
    public class AppTestContext : DbContext
    {
        public DbSet<LocalUser> User { get; set; }

        public AppTestContext(DbContextOptions<AppTestContext> options) : base(options) { }

        public void SeedData()
        {
            if (!this.User.Any())
            {
                var testUser = new LocalUser
                {
                    Username = "testi",
                    Password = "testi123!DE",
                    Name = "testi",
                    Role = "admin",
                };

                this.User.Add(testUser);
                this.SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelbuider)
        {
            modelbuider.Entity<LocalUser>().ToTable("User");
        }
    }
}
