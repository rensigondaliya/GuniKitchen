using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace GuniKitchen.Web.Models
{
    public class MyIdentityRole
       : IdentityRole<Guid>
    {

        [Display(Name = "Description")]
        [StringLength(100, ErrorMessage = "{0} cannot have more than {1} characters.")]
        public string Decription { get; set; }

    }
}
