using Kinetq.ServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kinetq.ServiceProvider.Builders;

public class ComposerBuilder<TResult>
{
    private readonly IList<IServiceBuilder> _serviceBuilders = new List<IServiceBuilder>();
    private readonly ISessionManager _sessionManager;
    private readonly IServiceProvider _serviceProvider;

    private IDbContextTransaction _transaction = null;
    private Func<TResult> _resultFunc;

    public ComposerBuilder(IServiceProvider serviceProvider, ISessionManager sessionManager)
    {
        _serviceProvider = serviceProvider;
        _sessionManager = sessionManager;
    }

    public ServiceBuilder<TDto, TId, TResult> For<TDto, TId>()
    {
        Type type = typeof(IService<TDto, TId>);

        var serviceType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetExportedTypes())
            .FirstOrDefault(p => p.IsInterface && type.IsAssignableFrom(p));

        IService<TDto, TId> service =
            (IService<TDto, TId>)_serviceProvider
                .GetService(serviceType);

        var serviceBuilder = new ServiceBuilder<TDto, TId, TResult>(service, this, _transaction);
        _serviceBuilders.Add(serviceBuilder);

        return serviceBuilder;
    }

    public void WithTransactionFor(string sessionName)
    {
        var session = _sessionManager.GetSessionFrom(sessionName);
        _transaction = session.Database.BeginTransaction();
    }

    public ComposerBuilder<TResult> Arrange<T1>(Func<T1, TResult> func)
    {
        var results = _serviceBuilders.SelectMany(x => x.Results);
        Task.WaitAll(results.ToArray());

        T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;

        _resultFunc = () => func(arg);
        return this;
    }

    public ComposerBuilder<TResult> Arrange<T1, T2>(Func<T1, T2, TResult> func)
    {
        var results = _serviceBuilders.SelectMany(x => x.Results);
        Task.WaitAll(results.ToArray());

        T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;
        T2 arg2 = ((Task<T2>)results.First(x => x.GetType() == typeof(Task<T2>))).Result;

        _resultFunc = () => func(arg, arg2);
        return this;
    }

    public ComposerBuilder<TResult> Arrange<T1, T2, T3>(Func<T1, T2, T3, TResult> func)
    {
        var results = _serviceBuilders.SelectMany(x => x.Results);
        Task.WaitAll(results.ToArray());

        T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;
        T2 arg2 = ((Task<T2>)results.First(x => x.GetType() == typeof(Task<T2>))).Result;
        T3 arg3 = ((Task<T3>)results.First(x => x.GetType() == typeof(Task<T3>))).Result;

        _resultFunc = () => func(arg, arg2, arg3);
        return this;
    }

    public ComposerBuilder<TResult> Arrange<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> func)
    {
        var results = _serviceBuilders.SelectMany(x => x.Results);
        Task.WaitAll(results.ToArray());

        T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;
        T2 arg2 = ((Task<T2>)results.First(x => x.GetType() == typeof(Task<T2>))).Result;
        T3 arg3 = ((Task<T3>)results.First(x => x.GetType() == typeof(Task<T3>))).Result;
        T4 arg4 = ((Task<T4>)results.First(x => x.GetType() == typeof(Task<T4>))).Result;

        _resultFunc = () => func(arg, arg2, arg3, arg4);
        return this;
    }

    public TResult Execute()
    {
        return _resultFunc();
    }
}