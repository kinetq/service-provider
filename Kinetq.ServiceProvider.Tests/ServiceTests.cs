using Kinetq.EntityFrameworkService.Builders;
using Kinetq.EntityFrameworkService.Interfaces;
using Kinetq.EntityFrameworkService.Tests.Models;
using Kinetq.EntityFrameworkService.Tests.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kinetq.EntityFrameworkService.Tests
{
    public class ServiceTests : ATests
    {
        private async Task AddCustomer()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = await session.GetSessionFrom(SessionKey);

            var dbSet = context.Set<Customer>();
            var newCustomer = new Customer
            {
                FirstName = "Sam",
                LastName = "Sinno",
                Id = 1,
                Utilities = Utilities.CableInternet | Utilities.Electricity,
                Orders = new List<Order>
                {
                    new Order
                    {
                        Id = 1,
                        Name = "Test Order"
                    },
                    new Order
                    {
                        Id = 2,
                        Name = "Test Order"
                    }
                }
            };

            dbSet.Add(newCustomer);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task CheckAnyByFilterBuilder()
        {
            await AddCustomer();
            var service = ServiceProvider.GetService<ICustomerService>();
            
            var hasCustomers =
                await service.GetByFilters()
                    .Where(Filter.Of(nameof(CustomerService.NameEquals), "Sam"))
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
                    .Where(Filter.Of(nameof(CustomerService.NameEquals), "Sam"))
                    .Entities();

            Assert.NotNull(customers);
            Assert.Equal(1, customers.Count);
        }

        [Fact]
        public async Task UpdateOneToMany()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = await session.GetSessionFrom(SessionKey);
            var service = ServiceProvider.GetService<ICustomerService>();

            await AddCustomer();

            var customerDto = await service.GetAsync(1);
            customerDto.OrderIds = new List<int>();
            customerDto.FirstName = "Tom";

            var success = await service.UpdateAsync(customerDto);
            var customer = context.Set<Customer>().Include("Orders").First(x => x.Id == 1);

            Assert.NotNull(success);
            Assert.Equal("Tom", customer.FirstName);
            Assert.Empty(customer.Orders);
        }
    }
}