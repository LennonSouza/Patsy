using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Patsy.Contratos.Interfaces;
using Patsy.Models;
using Patsy.ViewModels;
using System.Data;

namespace Patsy.Controllers;

[Authorize]
public class UsuarioController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UsuarioController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserWithRolesViewModel>>> Usuarios()
    {
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);

        var currentUserLevel = currentUser.HierarchyLevel;
        var currentUserSector = currentUser.Sector;

        // Filtrar usuários com base no nível hierárquico e setor
        List<UserWithRolesViewModel> userWithRolesViewModels;

        if (currentUserLevel == 1) // SuperAdmin
        {
            // SuperAdmin pode ver todos os usuários
            userWithRolesViewModels = await _userManager.Users
                .Select(user => new UserWithRolesViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result.ToList()
                })
                .ToListAsync();
        }
        else if (currentUserLevel == 2) // Admin
        {
            // Admin pode ver todos os usuários
            userWithRolesViewModels = await _userManager.Users
                .Where(user => user.Sector == currentUserSector && user.HierarchyLevel > currentUserLevel)
                .Select(user => new UserWithRolesViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result.ToList()
                })
                .ToListAsync();
        }
        else if (currentUserLevel == 3) // Gerente
        {
            // Gerente pode ver apenas usuários do seu setor
            userWithRolesViewModels = await _userManager.Users
                .Where(user => user.Sector == currentUserSector && user.HierarchyLevel > currentUserLevel)
                .Select(user => new UserWithRolesViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = _userManager.GetRolesAsync(user).Result.ToList()
                })
                .ToListAsync();
        }
        else // Funcionário
        {
            // Funcionário não pode ver outros usuários
            userWithRolesViewModels = new List<UserWithRolesViewModel>();
        }

        return View(userWithRolesViewModels);
    }

    //[HttpGet]
    //public async Task<ActionResult<IEnumerable<ApplicationUser>>> Usuarios()
    //{
    //    List<UserWithRolesViewModel> userWithRolesViewModels = await _userManager.Users
    //    .Select(user => new UserWithRolesViewModel
    //    {
    //        Id = user.Id,
    //        UserName = user.UserName,
    //        Email = user.Email,
    //        Roles = _userManager.GetRolesAsync(user).Result.ToList()
    //    })
    //    .ToListAsync();

    //    return View(userWithRolesViewModels);
    //}

    //[HttpGet]
    //public async Task<ActionResult<ApplicationUser>> User(string id)
    //{
    //    ApplicationUser user = await _unitOfWork.UserRepository.IdAsync(id);

    //    if (user is null) return View();

    //    return View(user);
    //}

    [HttpGet]
    public async Task<IActionResult> AddUser()
    {
        var currentUser = await _userManager.GetUserAsync(HttpContext.User);

        // Verifica se o usuário atual é válido
        if (currentUser is null)
        {
            return View();
        }

        var roles = await _roleManager.Roles.ToListAsync();

        var availableRoles = roles
                                 .Where(role =>
                                    (currentUser.HierarchyLevel == 1 && role.Name != "SuperAdmin") || // SuperAdmin (Nível 1) pode ver tudo, exceto SuperAdmin
                                    (currentUser.HierarchyLevel == 2 && (role.HierarchyLevel == 3 || role.HierarchyLevel == 4)) || // Admin (Nível 2) pode ver roles de nível 3 e 4
                                    (currentUser.HierarchyLevel == 3 && role.HierarchyLevel == 4)) // Gerente (Nível 3) pode ver apenas a role de nível 4
                                 .ToList();

        ViewBag.Roles = availableRoles; // Passa as roles filtradas para a view
        return View(new ApplicationUser());

        //var roles = await _roleManager.Roles.ToListAsync(); // Obtém todas as roles disponíveis
        //ViewBag.Roles = roles; // Passa as roles para a view
        //return View(new ApplicationUser());
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationUser>> AddUser(ApplicationUser user, string password, List<string> roles)
    {
        if (!roles.Any())
        {
            return View();
        }

        var currentUser = await _userManager.GetUserAsync(HttpContext.User);

        // Verifica se o usuário atual é válido
        if (currentUser == null)
        {
            return View(); // Retorna 401 se o usuário não estiver autenticado
        }

        // Verificar se o novo usuário está no mesmo setor e no nível hierárquico apropriado
        if(user.HierarchyLevel > 1)
        {
            if (currentUser.Sector != user.Sector || user.HierarchyLevel < currentUser.HierarchyLevel)
            {
                ModelState.AddModelError("", "Você só pode criar usuários abaixo do seu nível e no seu setor.");
                return View(user); // Retorna à view com erro
            }
        }

        // Cria o usuário
        IdentityResult result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            if (roles != null && roles.Any())
            {
                await _userManager.AddToRolesAsync(user, roles);
            }

            return RedirectToAction("Usuarios", "Usuario");
        }

        // Se falhar, retorne os erros
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(user); // Retorna à view com os erros

        //IdentityResult result = await _userManager.CreateAsync(user, password);
        //if (result.Succeeded)
        //{
        //    if (roles != null && roles.Any())
        //    {
        //        await _userManager.AddToRolesAsync(user, roles);
        //    }

        //    return RedirectToAction("Usuarios", "Usuario");
        //}

        //// Se falhar, retorne os erros
        //foreach (var error in result.Errors)
        //{
        //    ModelState.AddModelError("", error.Description);
        //}

        //return View();
    }

    [HttpGet]
    public async Task<IActionResult> UpdateUser(string id)
    {
        ApplicationUser user = await _unitOfWork.UserRepository.IdAsync(id);
        if (user is null)
        {
            return View();
        }

        // Obter todas as roles disponíveis
        var availableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        var userWithRolesViewModel = new UserWithRolesViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = (await _userManager.GetRolesAsync(user)).ToList(),
            AvailableRoles = availableRoles // Passar as roles disponíveis para a view
        };

        return View(userWithRolesViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUser(string id, UserWithRolesViewModel updatedUser)
    {
        if (id != updatedUser.Id)
        {
            return View();
        }

        ApplicationUser user = await _unitOfWork.UserRepository.IdAsync(id);
        if (user is null)
        {
            return View();
        }

        user.Email = updatedUser.Email;
        user.UserName = updatedUser.UserName;

        // Atualizar usuário no banco
        IdentityResult result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            // Remover todas as roles atuais
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Adicionar as novas roles selecionadas
            if (updatedUser.Roles != null && updatedUser.Roles.Any())
            {
                await _userManager.AddToRolesAsync(user, updatedUser.Roles);
            }

            return RedirectToAction("Usuarios");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(updatedUser); // Retorna a view com os erros
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(string id)
    {
        ApplicationUser user = await _unitOfWork.UserRepository.IdAsync(id);

        if (user is null) return NotFound();

        return View(user);
    }


    [HttpPost]
    public async Task<IActionResult> ConfirmDeleteUser(string id)
    {
        ApplicationUser user = await _unitOfWork.UserRepository.IdAsync(id);

        if (user is null) return NotFound();

        IdentityResult result = await _userManager.DeleteAsync(user);

        if (result.Succeeded) return RedirectToAction("Usuarios");

        return View(user);
    }

    //[HttpGet]
    //public async Task<IActionResult> AddRoleToUser(string id)
    //{
    //    var user = await _unitOfWork.UserRepository.IdAsync(id);
    //    if (user == null) return NotFound();

    //    // Obter todas as roles disponíveis
    //    var availableRoles = await _unitOfWork.RoleService.RolesAsync(); // Método que você deve implementar

    //    var model = new AddRoleToUserViewModel
    //    {
    //        UserId = user.Id,
    //        AvailableRoles = availableRoles
    //    };

    //    return View(model);
    //}


    //[HttpPost]
    //public async Task<IActionResult> AddRoleToUser(string id, string role)
    //{
    //    IdentityResult result = await _unitOfWork.RoleService.AddUserToRoleAsync(id, role);

    //    if (result.Succeeded) return RedirectToAction("Usuarios");

    //    return RedirectToAction("Index", "Home");
    //}
}