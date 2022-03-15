using GuniKitchen.Web.Models;
using GuniKitchen.Web.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace GuniKitchen.Web.Data
{
    public static class ApplicationDbContextSeed
    {

        public static async Task SeedIdentityRolesAsync(RoleManager<MyIdentityRole> rolemanager)
        {
            foreach (MyIdentityRoleNames role in Enum.GetValues(typeof(MyIdentityRoleNames)))
            {
                string rolename = role.ToString();
                if (!await rolemanager.RoleExistsAsync(rolename))
                {
                    await rolemanager.CreateAsync(
                        new MyIdentityRole { Name = rolename });
                }
            }
        }


        public static async Task SeedIdentityUserAsync(UserManager<MyIdentityUser> usermanager)
        {
            MyIdentityUser user;
            
            user = await usermanager.FindByNameAsync("admin@gunikitchen.com");
            if (user == null)
            {
                user = new MyIdentityUser()
                {
                    UserName = "admin@gunikitchen.com",
                    Email = "admin@gunikitchen.com",
                    EmailConfirmed = true,
                    DisplayName = "The Admin User",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    Gender = MyIdentityGenders.Female,
                    IsAdminUser = true
                };
                await usermanager.CreateAsync(user, password: "Asdf@123");
                await usermanager.AddToRolesAsync(user, new string[] {
                    MyIdentityRoleNames.Administrator.ToString(),
                    MyIdentityRoleNames.Manager.ToString()
                });
            }

            user = await usermanager.FindByNameAsync("manager@gunikitchen.com");
            if (user == null)
            {
                user = new MyIdentityUser()
                {
                    UserName = "manager@gunikitchen.com",
                    Email = "manager@gunikitchen.com",
                    EmailConfirmed = true,
                    DisplayName = "The Manager",
                    DateOfBirth = new DateTime(2000, 1, 1),
                    Gender = MyIdentityGenders.Male,
                    IsAdminUser = true
                };
                await usermanager.CreateAsync(user, password: "Asdf@123");
                await usermanager.AddToRolesAsync(user, new string[] {
                    MyIdentityRoleNames.Manager.ToString()
                });
            }

        }

    }
}
