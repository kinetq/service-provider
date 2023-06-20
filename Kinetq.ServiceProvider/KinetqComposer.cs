using System.ComponentModel.DataAnnotations;
using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Interfaces;

namespace Kinetq.ServiceProvider;

public class KinetqComposer : IKinetqComposer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IComposer> _composers;
    private readonly ISessionManager _sessionManager;
    private readonly IEnumerable<IKinetqService> _services;

    public KinetqComposer(
        IEnumerable<IComposer> composers, 
        ISessionManager sessionManager, 
        IEnumerable<IKinetqService> services)
    {
        _composers = composers;
        _sessionManager = sessionManager;
        _services = services;
    }

    public ComposerBuilder<TResult> Compose<TResult>()
    {
        return new ComposerBuilder<TResult>(_sessionManager, _services);
    }

    public T ComposeWith<T>()
    {
        return (T)_composers.First(x => x.GetType() == typeof(T));
    }
}