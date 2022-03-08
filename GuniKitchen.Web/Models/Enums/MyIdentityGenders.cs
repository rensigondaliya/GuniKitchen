using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GuniKitchen.Web.Models.Enums
{
    public enum MyIdentityGenders
    {
        [Display(Name = "Male")]
        Male,

        [Display(Name = "Female")]
        Female,

        [Display(Name = "Third Gender")]
        ThirdGender
    }
}
