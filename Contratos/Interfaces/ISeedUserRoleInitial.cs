namespace Patsy.Contratos.Interfaces;

public interface ISeedUserRoleInitial
{
    Task SeedRolesAsync();
    Task SeedUsersAsync();
}