using Microsoft.EntityFrameworkCore;

namespace Stock.API.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Stock> Stock { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().HasData(
                new Stock
                {
                    ProductId = 1,
                    Count = 100,
                }
            );
        }
    }
}
