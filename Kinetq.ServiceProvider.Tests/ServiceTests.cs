using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Dtos;
using Kinetq.ServiceProvider.Tests.Filters;
using Kinetq.ServiceProvider.Tests.Models;
using Kinetq.ServiceProvider.Tests.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kinetq.ServiceProvider.Tests
{
    public class ServiceTests : ATests
    {
        [Fact]
        public async Task CheckAnyByFilterBuilder()
        {
            await AddCustomer();
            var service = ServiceProvider.GetService<ICustomerService>();
            
            var hasCustomers =
                await service.GetByFilters()
                    .Where(Filter.Of(nameof(CustomerFilters.NameEquals), "Sam"))
                    .Any();

            Assert.True(hasCustomers);
        }

        [Fact]
        public async Task GetByFilterBuilder()
        {
            await AddCustomer();
            var service = ServiceProvider.GetService<ICustomerService>();

            var customers =
                await service.GetByFilters()
                    .Where(Filter.Of(nameof(CustomerFilters.NameEquals), "Sam"))
                    .Entities();

            Assert.NotNull(customers);
            Assert.Equal(1, customers.Count);
        }

        [Fact]
        public async Task UpdateOneToMany()
        {
            await AddCustomer();
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = session.GetSessionFrom(SessionKey);
            var service = ServiceProvider.GetService<ICustomerService>();

            var customerDto = await service.GetAsync(1);
            customerDto.OrderIds = new List<int>();
            customerDto.FirstName = "Tom";

            var success = await service.UpdateAsync(customerDto);
            var customer = context.Set<Customer>().Include("Orders").First(x => x.Id == 1);

            Assert.NotNull(success);
            Assert.Equal("Tom", customer.FirstName);
            Assert.Empty(customer.Orders);
        }

        [Fact]
        public async Task UpsertCreateAndUpdate()
        {
            await AddCustomer();

            var service = ServiceProvider.GetService<ICustomerService>();
            var customer1 = await service.GetAsync(1);

            customer1.FirstName = "John";

            var customer2 = new CustomerDto()
            {
                FirstName = "Harry",
                LastName = "Newman",
                Utilities = Utilities.Electricity
            };

            var customers = new List<CustomerDto>()
            {
                customer1,
                customer2,
            };

            var result = await service.UpsertAsync(customers);
            Assert.NotNull(result);
            Assert.NotEqual( 0, result[1].Id);
            Assert.NotEqual(0, result[0].Id);

            Assert.Equal("John", result.Single(x => x.Id.Equals(1)).FirstName);
        }
    }
}