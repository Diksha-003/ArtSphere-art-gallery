using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Project.DTO;
using Project.Models;

namespace newUser.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Artist> artists { get; set; }
    
        public DbSet<ApplicationUser> users { get; set; }

        public DbSet<ArtistImage> ArtistImages { get; set; }

        public DbSet<Exhb> exhb { get; set; }

        public DbSet<UserView> userViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map UserView to your SQL view
            modelBuilder.Entity<UserView>().ToTable("aspnetusers");
        }


    }
}
