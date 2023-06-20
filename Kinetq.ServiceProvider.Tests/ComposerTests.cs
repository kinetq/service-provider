using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Composers;
using Kinetq.ServiceProvider.Tests.Dtos;
using Kinetq.ServiceProvider.Tests.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kinetq.ServiceProvider.Tests;

public class ComposerTests : ATests
{
    [Fact]
    public async Task Can_Compose_Multiple_Services()
    {
        await AddCustomer();
        var composer = ServiceProvider.GetService<IKinetqComposer>();

       var result = 
           composer.Compose<int>()
            .For<CustomerDto, int>()
            .Operation(service => service.GetAllAsync())
            .Arrange((List<CustomerDto> customers) => customers.Count)
            .Execute();

       Assert.Equal(1, result);
    }
    
    [Fact]
    public async Task Can_Compose_With_Composer()
    {
        await AddCustomer();
        var composer = ServiceProvider.GetService<IKinetqComposer>();

       var result = 
           composer.ComposeWith<CustomerCountComposer>()
               .Execute();

       Assert.Equal(1, result);
    }
}