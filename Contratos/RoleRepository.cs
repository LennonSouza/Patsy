using Microsoft.AspNetCore.Identity;
using Patsy.Contratos.Interfaces;
using Patsy.Data;
using Patsy.Models;

namespace Patsy.Contratos;

public class RoleRepository : Repository<RoleManager<ApplicationRole>>, IRoleRepository
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleRepository(ApplicationDbContext context, RoleManager<ApplicationRole> roleManager) : base(context) => _roleManager = roleManager;

    public async Task<ApplicationRole> NameAsync(string roleName) => await _roleManager.FindByNameAsync(roleName);
}