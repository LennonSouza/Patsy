namespace Patsy.Contratos.Interfaces;

public interface IRepository<T>
{
    Task Adicionar(T entity);
    Task AdicionarRange(IEnumerable<T> entities);
    Task Atualizar(T entity);
    Task AtualizarRange(IEnumerable<T> entities);
    Task Excluir(T entity);
    Task ExcluirRange(IEnumerable<T> entities);
}