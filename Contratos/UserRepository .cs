using Microsoft.AspNetCore.Identity;
using Patsy.Contratos.Interfaces;
using Patsy.Data;
using Patsy.Models;

namespace Patsy.Contratos;

public class UserRepository : Repository<UserManager<ApplicationUser>>, IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context) => _userManager = userManager;

    public IQueryable<ApplicationUser> Usuarios() => _context.Users;

    public async Task<ApplicationUser> EmailAsync(string email) => await _userManager.FindByEmailAsync(email);
    public async Task<ApplicationUser> IdAsync(string id) => await _context.Users.FindAsync(id);
}