using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Dtos;

namespace Kinetq.ServiceProvider.Tests.Composers;

public class CustomerCountComposer : ComposerBuilder<int>, IComposer
{
    public CustomerCountComposer(IServiceProvider serviceProvider, ISessionManager sessionManager) : base(serviceProvider, sessionManager)
    {
        For<CustomerDto, int>()
            .Operation(service => service.GetAllAsync())
            .Arrange((List<CustomerDto> customers) => customers.Count);
    }
}