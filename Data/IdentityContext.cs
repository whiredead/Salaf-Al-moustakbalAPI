using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SalafAlmoustakbalAPI.Models;

namespace SalafAlmoustakbalAPI.Data
{
    public class IdentityContext : IdentityDbContext<User>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        public DbSet<Bar> Bars { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Domicile> Domiciles { get; set; }
        public DbSet<ville> Villes { get; set; }
        public DbSet<StatutOccupationLogement> statut { get; set; }
        public DbSet<Dossier> Dossiers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

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

            modelBuilder.Entity<Client>()
               .HasMany(c => c.dossiers)
               .WithOne(d => d.client)
               .HasForeignKey(d => d.ClientId);

            modelBuilder.Entity<User>()
               .HasMany(u => u.dossiers)
               .WithOne(d => d.user)
               .HasForeignKey(d => d.UserId);

            /*modelBuilder.Entity<Client>()
               .HasOne(c => c.domicile)
               .WithOne(d => d.client)
               .HasForeignKey<Client>(c => c.domicileId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);*/

            modelBuilder.Entity<Domicile>()
                .HasOne(d => d.ville)
                .WithOne()
                .HasForeignKey<Domicile>( d => d.VilleId)
                .IsRequired(false);

            modelBuilder.Entity<Domicile>()
                .HasIndex(d => d.VilleId)
                .IsUnique(false);

            modelBuilder.Entity<Client>()
               .HasOne(c => c.statutOccupationLogement)
               .WithOne()
               .HasForeignKey<Client>(c => c.StatutOccupationLogementId)
               .IsRequired(false);

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.StatutOccupationLogementId)
                .IsUnique(false);


            modelBuilder.Entity<Client>()
                .HasOne(cl => cl.domicile)
                .WithOne()
                .HasForeignKey<Client>(cl => cl.domicileId);

            

            base.OnModelCreating(modelBuilder);




        }
    }
}
