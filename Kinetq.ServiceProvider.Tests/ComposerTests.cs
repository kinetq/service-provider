using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Composers;
using Kinetq.ServiceProvider.Tests.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kinetq.ServiceProvider.Tests;

public class ComposerTests : ATests
{
    [Fact]
    public async Task Can_Compose_With_Services()
    {
        await AddCustomer();
        var composer = ServiceProvider.GetService<IKinetqComposer>();

        var result =
            composer.Compose<int>()
             .For<CustomerDto, int>()
             .Operation(service =>
             {
                 return service.GetAllAsync();
             })
             .Arrange((List<CustomerDto> customers) =>
             {
                 return customers.Count;
             })
             .Execute();

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Can_Compose_With_Multiple_Services()
    {
        await AddCustomer();
        var composer = ServiceProvider.GetService<IKinetqComposer>();

        var result =
            composer.Compose<int>()
             .For<CustomerDto, int>()
             .Operation(service => service.GetAllAsync())
             .For<OrderDto, int>()
             .Operation(service => service.GetAllAsync())
             .Arrange((List<CustomerDto> customers, List<OrderDto> orders) =>
             {
                 return customers.Count + orders.Count;
             })
             .Execute();

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task Can_Compose_With_Multiple_Services_And_Transaction()
    {
        await AddCustomer();
        var composer = ServiceProvider.GetService<IKinetqComposer>();

        var result =
            composer.Compose<CustomerDto>()
                .For<OrderDto, int>()
                .Operation(service =>
                {
                    var order = new OrderDto()
                    {
                        Name = "My Test Order"
                    };

                    return service.CreateAsync(order);
                })
                .For<CustomerDto, int>()
                .Operation((IService<CustomerDto, int> service, OrderDto order) =>
                {
                    return service.CreateAsync(new CustomerDto() { FirstName = "Bob", OrderIds = new List<int>() { order.Id } });
                })
                .Arrange((CustomerDto customer) => customer)
                .Execute();

        Assert.Equal(1, result.OrderIds.Count());
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