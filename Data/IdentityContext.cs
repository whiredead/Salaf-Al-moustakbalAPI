using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectPfe.Models;

namespace ProjectPfe.Data
{
    public class IdentityContext : IdentityDbContext<User>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Menu> Menus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary keys for Identity entities
            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.LoginProvider, p.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });
            modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(p => p.Id);
            modelBuilder.Entity<IdentityUserToken<string>>().HasKey(p => new { p.UserId, p.LoginProvider, p.Name });
            modelBuilder.Entity<IdentityRoleClaim<string>>().HasKey(p => p.Id);

            // Your other configurations
            modelBuilder.Entity<Bar>()
               .HasMany(b => b.Menus)
               .WithOne(m => m.Bar)
               .HasForeignKey(m => m.BarId);
        }
    }
}
