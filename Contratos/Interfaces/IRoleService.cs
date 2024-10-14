using Microsoft.AspNetCore.Identity;

namespace Patsy.Contratos.Interfaces;

public interface IRoleService
{
    Task<List<string>> RolesAsync();
    Task<IdentityResult> AddUserToRoleAsync(string userId, string role);
}