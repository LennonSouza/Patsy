using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Patsy.Contratos.Interfaces;
using Patsy.Models;

namespace Patsy.Contratos;

public class RoleService : IRoleService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<List<string>> RolesAsync() => await _roleManager.Roles.Select(r => r.Name).ToListAsync();

    public async Task<IdentityResult> AddUserToRoleAsync(string userId, string role)
    {
        ApplicationUser user = await _userManager.FindByIdAsync(userId);
        if (user is null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

        return await _userManager.AddToRoleAsync(user, role);
    }
}