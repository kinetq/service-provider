namespace Kinetq.ServiceProvider.Interfaces;

public interface IServiceBuilder
{
    IList<Task> Results { get;  }
}