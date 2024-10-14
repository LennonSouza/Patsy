using Microsoft.AspNetCore.Identity;
using Patsy.Contratos.Interfaces;
using Patsy.Models;

namespace Patsy.Contratos;

public class SeedUserRoleInitial : ISeedUserRoleInitial
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;

    public SeedUserRoleInitial(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
        {
            ApplicationRole role = new();
            role.Name = "SuperAdmin";
            role.NormalizedName = "SUPERADMIN";
            role.HierarchyLevel = 1;
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            await _roleManager.CreateAsync(role);
        }
    }

    public async Task SeedUsersAsync()
    {
        var adminEmail = _configuration["AdminUser:Email"];
        var adminPassword = _configuration["AdminUser:Password"];

        if (await _userManager.FindByEmailAsync(adminEmail) == null)
        {
            ApplicationUser user = new()
            {
                UserName = adminEmail,
                Email = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                HierarchyLevel = 1
            };

            IdentityResult result = await _userManager.CreateAsync(user, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "SuperAdmin");
            }
        }
    }
}