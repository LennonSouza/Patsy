using Microsoft.AspNetCore.Identity;
using Patsy.Models;

namespace Patsy.Contratos.Interfaces;

public interface IUserRepository : IRepository<UserManager<ApplicationUser>>
{
    IQueryable<ApplicationUser> Usuarios();
    Task<ApplicationUser> EmailAsync(string email);
    Task<ApplicationUser> IdAsync(string id);
}