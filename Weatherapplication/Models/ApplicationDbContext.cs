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
        public DbSet<ItemMaster> ItemMaster { get; set; }
        public DbSet<QuotationDetail> QuotationDetail { get; set; }

        public DbSet<QuotationItemDetail> QuotationItemDetail { get; set; }
        public DbSet<UserRegistration> Users { get; set; }
        public DbSet<SalesDetail> SalesDetail { get; set; }
        public DbSet<SalesItemDetail> SalesItemDetail { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<QuotationItemDetail>()
        //        .HasOne(x => x.QuotationDetail)
        //        .WithMany(x => x.QuotationDetails)
        //        .HasForeignKey(x => x.QuotationId);
        //}
    }
}
