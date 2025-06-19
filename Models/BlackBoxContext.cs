using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;


namespace BlackBoxCheckApi.Models
{
    public class BlackBoxDbContext : DbContext
    {
        public DbSet<BoxedItem> boxedItems { get; set; }
        public DbSet<ItemsList> itemsLists { get; set; }

        public BlackBoxDbContext(DbContextOptions<BlackBoxDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BoxedItem>().HasKey(i => i.Id);
            modelBuilder.Entity<ItemsList>().HasKey(i => i.Id);
            modelBuilder.Entity<BoxedItem>(entity =>
            {
                entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            });

            modelBuilder.Entity<ItemsList>(entity =>
            {
                entity.Property(e => e.Name)
                .IsRequired();
                entity.Property(e => e.CreatedAt)
                .IsRequired();
                entity.Property(e => e.UpdateAt)
                .IsRequired();
            });
        }
    }
}
