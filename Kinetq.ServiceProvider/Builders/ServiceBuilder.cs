using Kinetq.ServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kinetq.ServiceProvider.Builders;

public class ServiceBuilder<TDto, TId, TResult> : IServiceBuilder
{
    private readonly IService<TDto, TId> _service;
    private readonly ComposerBuilder<TResult> _composer;
    private readonly IDbContextTransaction _transaction;
    private readonly IList<Task> _results = new List<Task>();
    public ServiceBuilder(IService<TDto, TId> service, ComposerBuilder<TResult> composer, IDbContextTransaction transaction)
    {
        _service = service;
        _composer = composer;
        _transaction = transaction;
    }

    public ComposerBuilder<TResult> Operation<T>(Func<IService<TDto, TId>, Task<T>> serviceFunc)
    {
        Results.Add(serviceFunc(_service));
        return _composer;
    }
    public ComposerBuilder<TResult> OperationWithTransaction<T>(Func<IService<TDto, TId>, IDbContextTransaction, Task<T>> serviceFunc)
    {
        Results.Add(serviceFunc(_service, _transaction));
        return _composer;
    }

    public IList<Task> Results => _results;
}