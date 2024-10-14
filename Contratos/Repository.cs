using Microsoft.EntityFrameworkCore;
using Patsy.Contratos.Interfaces;
using Patsy.Data;

namespace Patsy.Contratos;

public class Repository<T> : IRepository<T> where T : class
{
    protected ApplicationDbContext _context;

    public Repository(ApplicationDbContext contexto) => _context = contexto;

    public async Task Adicionar(T entity) => await _context.Set<T>().AddAsync(entity);

    public async Task AdicionarRange(IEnumerable<T> entities) => await _context.Set<T>().AddRangeAsync(entities);

    public async Task Atualizar(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        _context.Set<T>().Update(entity);
        await Task.CompletedTask;
    }

    public async Task AtualizarRange(IEnumerable<T> entities)
    {
        foreach (T entity in entities)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
        }
        await Task.CompletedTask;
    }

    public async Task Excluir(T entity)
    {
        _context.Set<T>().Remove(entity);
        await Task.CompletedTask;
    }

    public async Task ExcluirRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
        await Task.CompletedTask;
    }
}