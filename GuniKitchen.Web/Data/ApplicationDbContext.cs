using GuniKitchen.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace GuniKitchen.Web.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<MyIdentityUser, MyIdentityRole, Guid>
    {
        public DbSet<Category> Category { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Set the DEFAULT Constraint on the "CreatedAt" column for the "Categories" table.
            builder.Entity<Category>()
                   .Property(e => e.CreatedAt)
                   .HasDefaultValueSql("getdate()");
        }
    }
}
