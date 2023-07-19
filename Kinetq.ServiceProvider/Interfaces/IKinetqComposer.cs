using Kinetq.ServiceProvider.Builders;

namespace Kinetq.ServiceProvider.Interfaces;

public interface IKinetqComposer
{
    T ComposeWith<T>();
}