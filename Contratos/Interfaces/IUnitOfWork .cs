namespace Patsy.Contratos.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IRoleService RoleService { get; }
    Task Salvar();
}