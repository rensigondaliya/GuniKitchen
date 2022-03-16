using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Text.Json.Serialization;

namespace GuniKitchen.Web.Models
{
    [Table("ShoppingCartItems")]
    public class ShoppingCartItem
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }


        [Required]
        [ForeignKey(nameof(ShoppingCartItem.User))]
        public Guid UserId { get; set; }
        public MyIdentityUser User { get; set; }


        [Required]
        [ForeignKey(nameof(ShoppingCartItem.Product))]
        [Display(Name = "ID of the Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }


        [Required]
        [Display(Name = "Name of the Product")]
        public string ProductName { get; set; }


        [Display(Name = "Description")]
        public string ProductDescription { get; set; }


        [Display(Name = "Price per unit")]
        public decimal Price { get; set; }


        [Required]
        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; }


        [StringLength(150)]
        public string ProductImageFileUrl { get; set; }

    }
}
