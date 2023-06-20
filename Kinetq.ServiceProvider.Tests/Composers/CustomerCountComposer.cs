using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Dtos;

namespace Kinetq.ServiceProvider.Tests.Composers;

public class CustomerCountComposer : ComposerBuilder<int>, IComposer
{
    public CustomerCountComposer(ISessionManager sessionManager, IEnumerable<IKinetqService> services) 
        : base( sessionManager, services)
    {
        For<CustomerDto, int>()
            .Operation(service => service.GetAllAsync())
            .Arrange((List<CustomerDto> customers) => customers.Count);
    }
}