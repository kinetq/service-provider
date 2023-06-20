using System.ComponentModel.DataAnnotations;
using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Interfaces;

namespace Kinetq.ServiceProvider;

public class KinetqComposer : IKinetqComposer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IComposer> _composers;
    private readonly ISessionManager _sessionManager;

    public KinetqComposer(IServiceProvider serviceProvider, IEnumerable<IComposer> composers, ISessionManager sessionManager)
    {
        _serviceProvider = serviceProvider;
        _composers = composers;
        _sessionManager = sessionManager;
    }

    public ComposerBuilder<TResult> Compose<TResult>()
    {
        return new ComposerBuilder<TResult>(_serviceProvider, _sessionManager);
    }

    public T ComposeWith<T>()
    {
        return (T)_composers.First(x => x.GetType() == typeof(T));
    }
}