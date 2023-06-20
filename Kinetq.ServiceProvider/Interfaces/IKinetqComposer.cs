using Kinetq.ServiceProvider.Builders;

namespace Kinetq.ServiceProvider.Interfaces;

public interface IKinetqComposer
{
    ComposerBuilder<TResult> Compose<TResult>();
    T ComposeWith<T>();
}