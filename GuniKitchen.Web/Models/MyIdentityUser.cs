using GuniKitchen.Web.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GuniKitchen.Web.Models
{
    public class MyIdentityUser
        : IdentityUser<Guid>
    {

        [Display(Name = "Display Name")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [MinLength(2, ErrorMessage = "{0} should have at least {1} characters.")]
        [StringLength(60, ErrorMessage = "{0} cannot have more than {1} characters.")]
        public string DisplayName { get; set; }


        [Display(Name = "Date of Birth")]
        [Required]
        [PersonalData]                                      // for GDPR Complaince
        [Column(TypeName = "smalldatetime")]
        public DateTime DateOfBirth { get; set; }


        [Display(Name = "Is Admin User?")]
        [Required]
        public bool IsAdminUser { get; set; }


        [Required]
        [Display(Name = "Gender")]
        [PersonalData]
        public MyIdentityGenders Gender { get; set; }


        #region Navigation Properties to the ShoppingCart model

        public ICollection<ShoppingCartItem> ShoppingCartItems { get; set; }

        #endregion
    }
}
