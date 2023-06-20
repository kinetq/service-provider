using Kinetq.ServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Kinetq.ServiceProvider.Builders;

public class ComposerBuilder<TResult>
{
    private readonly IList<IServiceBuilder> _serviceBuilders = new List<IServiceBuilder>();
    private readonly IEnumerable<IKinetqService> _services; 
    private readonly ISessionManager _sessionManager;

    private IDbContextTransaction? _transaction = null;
    private Func<TResult> _resultFunc;

    public ComposerBuilder(ISessionManager sessionManager, IEnumerable<IKinetqService> services)
    {
        _sessionManager = sessionManager;
        _services = services;
    }

    public ServiceBuilder<TDto, TId, TResult> For<TDto, TId>()
    {
        Type type = typeof(IService<TDto, TId>);

        var service = _services.First(x => type.IsInstanceOfType(x));

        var previousResults = _serviceBuilders.SelectMany(x => x.Results).ToList();
        var serviceBuilder = new ServiceBuilder<TDto, TId, TResult>(service, this, _transaction, previousResults);
        _serviceBuilders.Add(serviceBuilder);

        return serviceBuilder;
    }

    public ComposerBuilder<TResult> WithTransactionFor(string sessionName)
    {
        var session = _sessionManager.GetSessionFrom(sessionName);
        _transaction = session.Database.BeginTransaction();

        return this;
    }

    public ComposerBuilder<TResult> Arrange<T1>(Func<T1, TResult> func)
    {
        _resultFunc = () =>
        {
            var results = _serviceBuilders.SelectMany(x => x.Results);
            Task.WaitAll(results.ToArray());

            T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;

            return func(arg);
        };
        return this;
    }

    public ComposerBuilder<TResult> Arrange<T1, T2>(Func<T1, T2, TResult> func)
    {
        _resultFunc = () =>
        {
            var results = _serviceBuilders.SelectMany(x => x.Results);
            Task.WaitAll(results.ToArray());

            T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;
            T2 arg2 = ((Task<T2>)results.First(x => x.GetType() == typeof(Task<T2>))).Result;

            return func(arg, arg2);
        };
        return this;
    }

    public ComposerBuilder<TResult> Arrange<T1, T2, T3>(Func<T1, T2, T3, TResult> func)
    {
        _resultFunc = () =>
        {
            var results = _serviceBuilders.SelectMany(x => x.Results);
            Task.WaitAll(results.ToArray());

            T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;
            T2 arg2 = ((Task<T2>)results.First(x => x.GetType() == typeof(Task<T2>))).Result;
            T3 arg3 = ((Task<T3>)results.First(x => x.GetType() == typeof(Task<T3>))).Result;

            return func(arg, arg2, arg3);
        };
        return this;
    }

    public ComposerBuilder<TResult> Arrange<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TResult> func)
    {
        _resultFunc = () =>
        {
            var results = _serviceBuilders.SelectMany(x => x.Results);
            Task.WaitAll(results.ToArray());

            T1 arg = ((Task<T1>)results.First(x => x.GetType() == typeof(Task<T1>))).Result;
            T2 arg2 = ((Task<T2>)results.First(x => x.GetType() == typeof(Task<T2>))).Result;
            T3 arg3 = ((Task<T3>)results.First(x => x.GetType() == typeof(Task<T3>))).Result;
            T4 arg4 = ((Task<T4>)results.First(x => x.GetType() == typeof(Task<T4>))).Result;

            return func(arg, arg2, arg3, arg4);
        };
        return this;
    }

    public TResult Execute()
    {
        if (_transaction != null)
        {
            _transaction.Commit();
        }

        return _resultFunc();
    }
}