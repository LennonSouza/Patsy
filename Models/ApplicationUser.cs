using Microsoft.AspNetCore.Identity;

namespace Patsy.Models;

public class ApplicationUser : IdentityUser
{
    public int HierarchyLevel { get; set; }
    public string Sector { get; set; }
}