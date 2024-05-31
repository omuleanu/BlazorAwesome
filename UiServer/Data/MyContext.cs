using Microsoft.EntityFrameworkCore;

namespace UiServer.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
        }

        public DbSet<Dinner> Dinners { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Chef> Chefs { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Lunch> Lunches { get; set; }
        public DbSet<TreeNode> TreeNodes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dinner>()
                .HasMany(o => o.Meals)
                .WithMany(o => o.Dinners);

            modelBuilder.Entity<Dinner>()
                .HasOne(o => o.BonusMeal)
                .WithMany();

            modelBuilder.Entity<Lunch>()
                .HasMany(o => o.Meals)
                .WithMany(o => o.Lunches);

            base.OnModelCreating(modelBuilder);
        }
    }
}