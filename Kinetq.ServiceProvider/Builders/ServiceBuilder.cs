using Kinetq.ServiceProvider.Helpers;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Kinetq.ServiceProvider.Builders;

public class ServiceBuilder<TDto, TId, TResult> : IServiceBuilder
{
    private readonly IKinetqService _service;
    private readonly ComposerBuilder<TResult> _composer;
    private readonly IDbContextTransaction? _transaction;
    private readonly IList<ServiceResult> _results = new List<ServiceResult>();
    private readonly IList<ServiceResult> _previousResults;
    public ServiceBuilder(
        IKinetqService service,
        ComposerBuilder<TResult> composer,
        IDbContextTransaction? transaction,
        IList<ServiceResult> previousResults)
    {
        _service = service;
        _composer = composer;
        _transaction = transaction;
        _previousResults = previousResults;
    }

    public ComposerBuilder<TResult> Operation<T>(Func<IService<TDto, TId>, Task<T>> serviceFunc)
    {
        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public ComposerBuilder<TResult> Operation<T, T1>(Func<IService<TDto, TId>, T1, Task<T>> serviceFunc)
    {
        var task = _previousResults.GetResult<T1>();
        Task.WaitAll(task);
        
        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service, task.Result),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public ComposerBuilder<TResult> Operation<T, T1, T2>(Func<IService<TDto, TId>, T1, T2, Task<T>> serviceFunc)
    {
        var task1 = _previousResults.GetResult<T1>();
        var task2 = _previousResults.GetResult<T2>();
        Task.WaitAll(task1, task2);

        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public ComposerBuilder<TResult> Operation<T, T1, T2, T3>(Func<IService<TDto, TId>, T1, T2, T3, Task<T>> serviceFunc)
    {
        var task1 = _previousResults.GetResult<T1>();
        var task2 = _previousResults.GetResult<T2>();
        var task3 = _previousResults.GetResult<T3>();
        Task.WaitAll(task1, task2, task3);

        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result, task3.Result),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T>(Func<IService<TDto, TId>, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service, _transaction),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T, T1>(Func<IService<TDto, TId>, T1, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        var task = _previousResults.GetResult<T1>();
        Task.WaitAll(task);

        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service, task.Result, _transaction),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T, T1, T2>(Func<IService<TDto, TId>, T1, T2, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        var task1 = _previousResults.GetResult<T1>();
        var task2 = _previousResults.GetResult<T2>();
        Task.WaitAll(task1, task2);

        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result, _transaction),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public ComposerBuilder<TResult> OperationWithTransaction<T, T1, T2, T3>(Func<IService<TDto, TId>, T1, T2, T3, IDbContextTransaction?, Task<T>> serviceFunc)
    {
        var task1 = _previousResults.GetResult<T1>();
        var task2 = _previousResults.GetResult<T2>();
        var task3 = _previousResults.GetResult<T3>();
        Task.WaitAll(task1, task2, task3);

        Results.Add(new ServiceResult()
        {
            OperationFunc = () => serviceFunc((IService<TDto, TId>)_service, task1.Result, task2.Result, task3.Result, _transaction),
            ReturnType = typeof(T)
        });
        return _composer;
    }

    public IList<ServiceResult> Results => _results;
}