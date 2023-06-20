using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Dtos;

namespace Kinetq.ServiceProvider.Tests.Services
{
    public interface IOrderService : IService<OrderDto, int>
    {
        
    }
}