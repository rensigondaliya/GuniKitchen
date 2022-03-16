using GuniKitchen.Web.Models;
using GuniKitchen.Web.Models.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using GuniKitchen.Web.Areas.Manage.ViewModels;

namespace GuniKitchen.Web.Data
{
    public class ApplicationDbContext
        : IdentityDbContext<MyIdentityUser, MyIdentityRole, Guid>
    {
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Products { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Calling HasConversion() on the Property "Size" on the "Product" model as shown below,
            // will create a ValueConverter<TModel, TProvider> instance and set it on the property.
            // By defining it, as shown below, can be use when multiple properties use the same conversion.
            // Check out: https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions for more info.
            var sizeConverter = new ValueConverter<ProductSizes, string>(
                v => v.ToString()
                , v => (ProductSizes)Enum.Parse(typeof(ProductSizes), v));

            // Set the DEFAULT Constraint on the "CreatedAt" column for the "Categories" table.
            builder.Entity<Category>()
                   .Property(e => e.CreatedAt)
                   .HasDefaultValueSql("getdate()");

            // Since Price is "decimal", set the precision and scale.
            // Precision is the total number of digits (including the decimal point).
            // Scale is the number of decimal places.
            builder.Entity<Product>()
                   .Property(e => e.Price)
                   .HasPrecision(precision: 6, scale: 2);
            builder.Entity<ShoppingCartItem>()
                   .Property(e => e.Price)
                   .HasPrecision(precision: 6, scale: 2);

            builder.Entity<Product>()
                   .Property(e => e.Size)
                   .HasConversion(sizeConverter);

        }

    }
}
