using Microsoft.AspNetCore.Identity;
using Patsy.Models;

namespace Patsy.Contratos.Interfaces;

public interface IRoleRepository : IRepository<RoleManager<ApplicationRole>>
{
    Task<ApplicationRole> NameAsync(string roleName);
}