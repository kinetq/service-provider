using AutoMapper;
using Kinetq.ServiceProvider.Config;
using Kinetq.ServiceProvider.Helpers;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Resolvers;
using Kinetq.ServiceProvider.Tests.Dtos;
using Kinetq.ServiceProvider.Tests.Models;
using Kinetq.ServiceProvider.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kinetq.ServiceProvider.Tests
{
    public abstract class ATests : IAsyncLifetime
    {
        protected IServiceProvider ServiceProvider;
        protected ServiceCollection ServiceCollection;

        protected string SessionKey => "Kinetq.EntityFrameworkService.Tests";

        public async Task InitializeAsync()
        {
            ServiceCollection = new ServiceCollection();

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            ServiceCollection.AddSingleton(mockHttpContextAccessor.Object);
            
            ServiceCollection.AddScoped<ICustomerService, CustomerService>();
            ServiceCollection.AddScoped<IKinetqService, CustomerService>();
            ServiceCollection.AddScoped<IKinetqService, OrderService>();
            ServiceCollection.AddDataServices("Kinetq.ServiceProvider.Tests");
            ServiceCollection.AddLogging(builder => builder.AddConsole());

            ServiceCollection.Add(new ServiceDescriptor(typeof(IMapper), provider =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.ConstructServicesUsing(provider.GetService);

                    cfg.CreateMap<OrderDto, Order>();
                    cfg.CreateMap<Order, OrderDto>();

                    cfg.CreateMap<CustomerDto, Customer>()
                        .ForMember(dst => dst.Orders,
                            opt => opt.MapFrom<CollectionProxyResolver<Order, int>, IEnumerable<int>>(x => x.OrderIds));

                    cfg.CreateMap<Customer, CustomerDto>()
                        .ForMember(dst => dst.OrderIds, opt => opt.MapFrom(src => src.Orders.Select(x => x.Id)));
                });

                return config.CreateMapper();
            }, ServiceLifetime.Singleton));

            ServiceProvider = ServiceCollection.BuildServiceProvider();

            var configManager = ServiceProvider.GetService<IConfigurationManager<EFOptions>>();
            
            var configBuilder =
                new DbContextOptionsBuilder()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());

            configManager.AddConfiguration(new EFOptions
            {
                Options = configBuilder.Options,
                SessionKey = SessionKey,
            });
        }

        protected async Task AddCustomer()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = session.GetSessionFrom(SessionKey);

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

        public async Task DisposeAsync()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = session.GetSessionFrom(SessionKey);

            context.Database.EnsureDeleted();
            session.CloseSessionOn(SessionKey);
        }
    }
}