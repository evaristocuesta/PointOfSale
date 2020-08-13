using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PointOfSale.WebAPI.Models;
using System;

namespace PointOfSale.WebAPI.Data
{
    public class PointOfSaleDbContext : IdentityDbContext<IdentityUser>
    {
        public PointOfSaleDbContext(DbContextOptions<PointOfSaleDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var hasher = new PasswordHasher<IdentityUser>();
            builder.Entity<IdentityUser>().HasData(
            new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "evaristocuesta",
                NormalizedUserName = "EVARISTOCUESTA",
                Email = "contact@evaristocuesta.com",
                NormalizedEmail = "CONTACT@EVARISTOCUESTA.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "changepass")
            }
        );
        }

        public DbSet<Client> Clients { get; set; }
    }
}
