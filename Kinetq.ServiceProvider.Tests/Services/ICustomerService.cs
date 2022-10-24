using Kinetq.EntityFrameworkService.Interfaces;
using Kinetq.EntityFrameworkService.Tests.Dtos;

namespace Kinetq.EntityFrameworkService.Tests.Services
{
    public interface ICustomerService : IService<CustomerDto, int>
    {
        
    }
}