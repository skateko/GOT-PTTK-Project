using Microsoft.AspNetCore.Identity;
using GOTHelperEng.Models;
namespace GOTHelperEng.Data
{
    public class MyIdentityDataInitializer
    {
        public static void SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedOneUser(userManager, "admin@localhost", "Admin1234!", "Admin");

        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new()
                {
                    Name = "Admin",
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Tourist").Result)
            {
                IdentityRole role = new()
                {
                    Name = "Tourist",
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Leader").Result)
            {
                IdentityRole role = new()
                {
                    Name = "Leader",
                };
                IdentityResult roleResult = roleManager.CreateAsync(role).Result;
            }
        }
        public static void SeedOneUser(UserManager<User> userManager, string name, string password, string role = null)
        {
            if (userManager.FindByNameAsync(name).Result == null)
            {
                Gender gender = new()
                {
                    GenderName = "M"
                };
                User user = new()
                {
                    UserName = name,
                    Email = name,
                    Gender = gender,
                    Name = "Admin",
                    Surname = "Adminski"
                };
                IdentityResult result = userManager.CreateAsync(user, password).Result;
                if (result.Succeeded && role != null)
                {
                    userManager.AddToRoleAsync(user, role).Wait();
                }
            }
        }
    }
}