using Kinetq.ServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kinetq.ServiceProvider.Builders;

public class ServiceBuilder<TDto, TId, TResult> : IServiceBuilder
{
    private readonly IKinetqService _service;
    private readonly ComposerBuilder<TResult> _composer;
    private readonly IDbContextTransaction? _transaction;
    private readonly IList<Task> _results = new List<Task>();
    private readonly IList<Task> _previousResults;
    public ServiceBuilder(
        IKinetqService service,
        ComposerBuilder<TResult> composer,
        IDbContextTransaction? transaction,
        IList<Task> previousResults)
    {
        _service = service;
        _composer = composer;
        _transaction = transaction;
        _previousResults = previousResults;
    }

    public ComposerBuilder<TResult> Operation<T>(Func<IService<TDto, TId>, Task<T>> serviceFunc)
    {
        Results.Add(serviceFunc((IService<TDto, TId>)_service));
        return _composer;
    }

    public ComposerBuilder<TResult> Operation<T, T1>(Func<IService<TDto, TId>, T1, Task<T>> serviceFunc)
    {
        var task = (Task<T1>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T1));
        Task.WaitAll(task);

        Results.Add(serviceFunc((IService<TDto, TId>)_service, task.Result));
        return _composer;
    }
    
    public ComposerBuilder<TResult> Operation<T, T1, T2>(Func<IService<TDto, TId>, T1, T2, Task<T>> serviceFunc)
    {
        var task1 = ((Task<T1>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T1)));
        var task2 = ((Task<T2>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T2)));
        Task.WaitAll(task1, task2);

        Results.Add(serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result));
        return _composer;
    }
    
    public ComposerBuilder<TResult> Operation<T, T1, T2, T3>(Func<IService<TDto, TId>, T1, T2, T3, Task<T>> serviceFunc)
    {
        var task1 = ((Task<T1>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T1)));
        var task2 = ((Task<T2>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T2)));
        var task3 = ((Task<T3>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T3)));
        Task.WaitAll(task1, task2, task3);

        Results.Add(serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result, task3.Result));
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T>(Func<IService<TDto, TId>, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        Results.Add(serviceFunc((IService<TDto, TId>)_service, _transaction));
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T, T1>(Func<IService<TDto, TId>, T1, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        var task = (Task<T1>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T1)); ;
        Task.WaitAll(task);

        Results.Add(serviceFunc((IService<TDto, TId>)_service, task.Result, _transaction));
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T, T1, T2>(Func<IService<TDto, TId>, T1, T2, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        var task1 = ((Task<T1>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T1)));
        var task2 = ((Task<T2>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T2)));
        Task.WaitAll(task1, task2);

        Results.Add(serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result, _transaction));
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T, T1, T2, T3>(Func<IService<TDto, TId>, T1, T2, T3, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        var task1 = ((Task<T1>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T1)));
        var task2 = ((Task<T2>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T2)));
        var task3 = ((Task<T3>)_previousResults.First(x => x.GetType().GenericTypeArguments[0] == typeof(T3)));
        Task.WaitAll(task1, task2, task3);

        Results.Add(serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result, task3.Result, _transaction));
        return _composer;
    }

    public IList<Task> Results => _results;
}