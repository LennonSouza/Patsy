using Microsoft.AspNetCore.Identity;
using Patsy.Contratos.Interfaces;
using Patsy.Data;
using Patsy.Models;

namespace Patsy.Contratos;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;
    private UserRepository _userRepository;
    private RoleRepository _roleRepository;
    private IRoleService _roleService;

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IRoleService roleService)
    {
        _context = context;
        _userManager = userManager;
        _roleService = roleService;
    }

    public IUserRepository UserRepository
    {
        get => _userRepository = _userRepository ?? new UserRepository(_context, _userManager);
    }

    public IRoleRepository RoleRepository
    {
        get => _roleRepository = _roleRepository ?? new RoleRepository(_context, _roleManager);
    }

    public IRoleService RoleService => _roleService;

    public async Task Salvar() => await _context.SaveChangesAsync();
    public async Task Dispose() => await _context.DisposeAsync();
}