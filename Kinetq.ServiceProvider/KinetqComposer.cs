using Kinetq.ServiceProvider.Interfaces;

namespace Kinetq.ServiceProvider;

public class KinetqComposer : IKinetqComposer
{
    private readonly IEnumerable<IComposer> _composers;

    public KinetqComposer(IEnumerable<IComposer> composers)
    {
        _composers = composers;
    }

    public T ComposeWith<T>()
    {
        return (T)_composers.First(x => x.GetType() == typeof(T));
    }
}