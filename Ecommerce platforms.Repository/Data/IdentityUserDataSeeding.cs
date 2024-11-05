
using Ecommerce_platforms.Repository.Data.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_platforms.Repository.Data
{
    public static class IdentityUserDataSeeding
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed Roles
            if (!roleManager.Roles.Any())
            {
                var roles = new List<IdentityRole>
                {
                   
                    new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                    new IdentityRole { Name = "User", NormalizedName = "USER" },
                    new IdentityRole{Name = "SubAdmin" ,NormalizedName = "SUBADMIN"}
                };
                
                foreach (var role in roles)
                {
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role.Name}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Seed Users
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    FName = "Mohamed",
                    LName = "Samir",
                    UserName = "MohamedSamir",
                    Email = "smsm@gmail.com"
                };

                var result = await userManager.CreateAsync(user, "Smsm99#$");
                if (result.Succeeded)
                {
                    // Assign roles to the user
                    var roleResult = await userManager.AddToRoleAsync(user, "Admin");
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to assign role to user: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    throw new Exception($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
