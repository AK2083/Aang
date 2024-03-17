using Aang.Model;
using Microsoft.EntityFrameworkCore;

namespace Aang.Database
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<LocalUser> User { get; set; }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
    }
}
