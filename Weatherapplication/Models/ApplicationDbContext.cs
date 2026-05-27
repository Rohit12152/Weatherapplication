using Microsoft.EntityFrameworkCore;

namespace Weatherapplication.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StudentDetails> StudentDetails { get; set; }
    }
}
