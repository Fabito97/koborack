using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using KoboRack.Model.Entities;

namespace KoboRack.Utility
{
    public class Seeder
    {
        public static async Task SeedRolesAndAdminUser(IServiceProvider serviceProvider)
        {
            using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
            using (var seededAdmin = serviceProvider.GetRequiredService<UserManager<AppUser>>())
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var adminRole = new IdentityRole("Admin");
                    await roleManager.CreateAsync(adminRole);
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    var userRole = new IdentityRole("User");
                    await roleManager.CreateAsync(userRole);
                }

                if (await seededAdmin.FindByNameAsync("Admin") == null)
                {
                    var admin = new AppUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        FirstName = "Admin",
                        LastName = "Suki",
                        Email = "admin@savi.com",
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        PhoneNumber = "1234567890",
                        CreatedAt = DateTime.UtcNow.Date,
                        ModifiedAt = DateTime.UtcNow.Date,
                        IsActive = true,
                        NormalizedEmail = "ADMIN@SAVI.COM",
                        UserName = "admin@savi.com",
                    };

                    var result = await seededAdmin.CreateAsync(admin, "Admin@1234");
                    if (result.Succeeded)
                    {
                        await seededAdmin.AddToRoleAsync(admin, "Admin");
                    }
                    else
                    {
                        // Handle user creation failure
                    }
                }
            }
        }
    }
}
