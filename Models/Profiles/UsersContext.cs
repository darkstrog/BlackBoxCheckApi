using Microsoft.EntityFrameworkCore;

namespace BlackBoxCheckApi.Models.Profiles
{
    public class UsersContext : DbContext
    {
        public DbSet<UserProfile> Users { get; set; }
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserProfile>()
                        .HasKey(User => User.Id);
            modelBuilder.Entity<UserProfile>()
                        .HasIndex(User => User.Login)
                        .IsUnique();
            modelBuilder.Entity<UserProfile>()
                        .Property(User => User.Password)
                        .IsRequired();
        }
    }
}
