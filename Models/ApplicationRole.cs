using Microsoft.AspNetCore.Identity;

namespace Patsy.Models;

public class ApplicationRole : IdentityRole
{
    public int HierarchyLevel { get; set; }

    // Construtor que aceita o nome do role
    public ApplicationRole(string roleName, int hierarchyLevel) : base(roleName)
    {
        HierarchyLevel = hierarchyLevel;
    }

    // Construtor padrão sem parâmetros
    public ApplicationRole() { }
}