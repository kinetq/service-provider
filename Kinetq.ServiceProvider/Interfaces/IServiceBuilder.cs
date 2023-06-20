using Kinetq.ServiceProvider.Models;

namespace Kinetq.ServiceProvider.Interfaces;

public interface IServiceBuilder
{
    IList<ServiceResult> Results { get;  }
}