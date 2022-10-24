using AutoMapper;
using Kinetq.EntityFrameworkService.Config;
using Kinetq.EntityFrameworkService.Helpers;
using Kinetq.EntityFrameworkService.Interfaces;
using Kinetq.EntityFrameworkService.Managers;
using Kinetq.EntityFrameworkService.Resolvers;
using Kinetq.EntityFrameworkService.Tests.Dtos;
using Kinetq.EntityFrameworkService.Tests.Models;
using Kinetq.EntityFrameworkService.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Kinetq.EntityFrameworkService.Tests
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


            ServiceCollection.AddSingleton<ISessionManager, SessionManager>();
            ServiceCollection.AddSingleton<IConfigurationManager<EFOptions>, ConfigurationManager>();
            ServiceCollection.AddScoped<ICustomerService, CustomerService>();
            ServiceCollection.AddDataServices("Kinetq.EntityFrameworkService.Tests");
            ServiceCollection.AddLogging(builder => builder.AddConsole());

            ServiceCollection.Add(new ServiceDescriptor(typeof(IMapper), provider =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.ConstructServicesUsing(provider.GetService);

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
                    .UseInMemoryDatabase(databaseName: "Add_writes_to_database");

            configManager.AddConfiguration(new EFOptions
            {
                Options = configBuilder.Options,
                SessionKey = SessionKey,
            });
        }

        public async Task DisposeAsync()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = await session.GetSessionFrom(SessionKey);

            context.Database.EnsureDeleted();
            session.CloseSessionOn(SessionKey);
        }
    }
}