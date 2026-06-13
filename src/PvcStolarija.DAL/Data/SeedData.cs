using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PvcStolarija.DAL.Models;

namespace PvcStolarija.DAL.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Administrator", "Kupac" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var adminUser = await userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@pvc-stolarija.rs",
                    Ime = "Admin",
                    Prezime = "Administrator",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newAdmin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(newAdmin, "Administrator");
            }
        }
    }
}
