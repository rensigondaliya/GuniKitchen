using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GuniKitchen.Web.Data;
using GuniKitchen.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuniKitchen.Web.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(
            UserManager<MyIdentityUser> userManager,
            SignInManager<MyIdentityUser> signInManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            #region Additional Properties as defined in MyIdentityUser Model

            [Display(Name = "Display Name")]
            [Required(ErrorMessage = "{0} cannot be empty.")]
            [MinLength(2, ErrorMessage = "{0} should have at least {1} characters.")]
            [StringLength(60, ErrorMessage = "{0} cannot have more than {1} characters.")]
            public string DisplayName { get; set; }

            [Display(Name = "Date of Birth")]
            [Required]
            public DateTime DateOfBirth { get; set; }

            [Display(Name = "Is Admin User?")]
            [Required]
            public bool IsAdminUser { get; set; }

            #endregion
        }

        private async Task LoadAsync(MyIdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                DisplayName = user.DisplayName,
                DateOfBirth = user.DateOfBirth,
                IsAdminUser = user.IsAdminUser
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            bool hasChangedDisplayName = false;
            if (Input.DisplayName != user.DisplayName)
            {
                user.DisplayName = Input.DisplayName;
                hasChangedDisplayName = true;
            }

            bool hasChangedDateOfBirth = false;
            if (Input.DateOfBirth != user.DateOfBirth)
            {
                user.DateOfBirth = Input.DateOfBirth;
                hasChangedDateOfBirth = true;
            }

            if(hasChangedDisplayName || hasChangedDateOfBirth)
            {
                _dbContext.SaveChanges();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated successfully!";
            return RedirectToPage();
        }
    }
}
