using Lab05_MaytaCuevas.Models;
using System.Collections;
using Lab04_MaytaAlberth.Repositories;
using Lab05_MaytaAlberth.UnitOfWork;
using Lab05_MaytaCuevas;

public class UnitOfWork : IUnitOfWork
{
    private readonly Hashtable _repositories;
    private readonly SistemaAcoContex _context;

    public UnitOfWork(SistemaAcoContex context)
    {
        _context = context;
        _repositories = new Hashtable();
    }

    public Task<int> Complete()
    {
        return _context.SaveChangesAsync();
    }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).Name;

        if (_repositories.ContainsKey(type))
        {
            return (IGenericRepository<TEntity>)_repositories[type];
        }

        var repositoryType = typeof(GenericRepository<>);
        var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

        if (repositoryInstance != null)
        {
            _repositories.Add(type, repositoryInstance);
            return (IGenericRepository<TEntity>)repositoryInstance;
        }

        throw new Exception($"Could not create repository instance for type {type}");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
