using BoardGameCompany.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoardGameCompany.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BoardGame> BoardGame { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<InventoryItem> InventoryItem { get; set; }
        public DbSet<UserComment> UserComment { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<UserCart> UserCart { get; set; }
        public DbSet<UserShippingInformation> UserShippingInformation { get; set; }
        public DbSet<UserBillingInformation> UserBillingInformation { get; set; }
        public DbSet<UserOrderInformation> UserOrderInformation { get; set; }
        public DbSet<UserOrderInformationPrice> UserOrderInformationPrice { get; set; }
        
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoardGame>()
                .HasMany(b => b.InventoryItems)
                .WithOne(i => i.BoardGame)
                .HasForeignKey(i => i.BoardGameID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<IdentityUserLogin<string>>().HasNoKey();
            modelBuilder.Entity<IdentityUserRole<string>>().HasNoKey();
            modelBuilder.Entity<IdentityUserToken<string>>().HasNoKey();
        }*/

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }*/
    }
}
