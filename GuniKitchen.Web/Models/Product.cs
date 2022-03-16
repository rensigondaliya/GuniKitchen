using GuniKitchen.Web.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuniKitchen.Web.Models
{
    [Table("Products")]
    public class Product
    {

        [Display(Name = "ID of the Product")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }


        [Display(Name = "Name of the Product")]
        [Required]
        [StringLength(80, ErrorMessage = "{0} cannot have more than {1} characters.")]
        [MinLength(3, ErrorMessage = "{0} should have a minimum of {1} characters.")]
        public string ProductName { get; set; }


        [Display(Name = "Description")]
        public string ProductDescription { get; set; }


        [Display(Name = "Price per unit")]
        [Required]
        [Range(0.0, 500.00, ErrorMessage = "{0} has to be between Rs. {1} and Rs. {2}")]
        public decimal Price { get; set; }


        [Required]
        [Display(Name = "Unit of Measure")]
        public string UnitOfMeasure { get; set; }


        /// <summary>
        ///     Size of the Product
        /// </summary>
        /// <remarks>
        ///     This is mapped to an enumeration of Sizes
        /// </remarks>
        [Display(Name = "Size")]
        [Column(TypeName = "varchar(20)")]
        public ProductSizes Size { get; set; }


        [StringLength(150)]
        public string ProductImageFileUrl { get; set; }


        [StringLength(60)]
        public string ProductImageContentType { get; set; }


        #region Navigational Properties to the Category Model

        [ForeignKey(nameof(Product.Category))]
        public short CategoryId { get; set; }

        public Category Category { get; set; }

        #endregion


        #region Navigational Properties to the ShoppingCartItem Model

        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        #endregion
    }
}
