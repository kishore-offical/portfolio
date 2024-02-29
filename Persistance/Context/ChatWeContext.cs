using ChatWe.Persistance.Configurations;
using ChatWe.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatWe.Persistance.Context
{
    public class ChatWeContext : IdentityDbContext<User>, IChatWeContext
    {
        public ChatWeContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable(name: "UserRole");
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomConfigurations).Assembly);
        }

        public DbSet<User> User { get; set; }
        public DbSet<Attachments> Attachments { get; set; }
        public DbSet<Conversations> Conversations { get; set; }
        public DbSet<Group> Group { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}