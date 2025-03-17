using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
      // Ensure the database is created and migrations are applied
        // This is handled by Entity Framework Core, so no need to do it here.

        // Step 1 Create Roles 
        string[] roleNames = {"Admin", "User"};
        foreach (var roleName in roleNames)
        {
            // Check if the role already exist
            if(!await roleManager.RoleExistsAsync(roleName))
            {
                //create the role if it doesn't exist
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }  
        //Step 2: Create Admin User
        var adminUser = new IdentityUser
        {
            UserName = "admin@example.com", // Admin username
            Email = "admin@example.com",  // Admin email
            EmailConfirmed = true,      // Skip email confirmation
        };

        string adminPassword = "Admin@123";  // Admin password
        var user = await userManager.FindByEmailAsync(adminUser.Email);

        //check if the admin user already exists 
        if (user == null)
        {
            // create the admin user 
            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (createResult.Succeeded)
            {
                //assign the "Admin" role to the admin user
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
        }
    }
}