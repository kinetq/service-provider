using System.Text;
using System.Text.Json;
using AutoFixture;
using AutoMapper;
using Kinetq.ServiceProvider.Config;
using Kinetq.ServiceProvider.Helpers;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Managers;
using Kinetq.ServiceProvider.Middleware;
using Kinetq.ServiceProvider.Resolvers;
using Kinetq.ServiceProvider.Tests.Dtos;
using Kinetq.ServiceProvider.Tests.Models;
using Kinetq.ServiceProvider.Tests.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Kinetq.ServiceProvider.Tests;

public class MiddlewareTests
{
    private readonly TestServer _testServer;
    private readonly IFixture _fixture = new Fixture();

    public MiddlewareTests()
    {
        var host = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddHttpContextAccessor();
                services.AddSingleton<ISessionManager, SessionManager>();
                services.AddSingleton<IConfigurationManager<EFOptions>, ConfigurationManager>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddDataServices("Kinetq.ServiceProvider.Tests");
                services.AddLogging(builder => builder.AddConsole());
                services.AddRouting();
                services.AddControllers();

                services.Add(new ServiceDescriptor(typeof(IMapper), provider =>
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
            })
            .Configure(app =>
            {
                var configManager = app.ApplicationServices.GetService<IConfigurationManager<EFOptions>>();

                var configBuilder =
                    new DbContextOptionsBuilder()
                        .UseInMemoryDatabase(databaseName: "Add_writes_to_database");

                configManager.AddConfiguration(new EFOptions
                {
                    Options = configBuilder.Options,
                    SessionKey = "Kinetq.EntityFrameworkService.Tests",
                });


                app.UseMiddleware<CloseSessionMiddleware>();
                app.UseRouting();
                app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            });

        _testServer = new TestServer(host);
    }

    [Fact]
    public async Task CustomerApi_Post_ProperlyClosesSession()
    {
        var customerDto = _fixture.Create<CustomerDto>();
        customerDto.OrderIds = new List<int>();

        var payload = JsonSerializer.Serialize(customerDto);
        var testClient = _testServer.CreateClient();
        var response = 
            await testClient.PostAsync("/customer", new StringContent(payload, Encoding.UTF8, "application/json"));

        var sessionManager = _testServer.Services.GetService<ISessionManager>();
        Assert.Equal(0, sessionManager.ContextSessions.Count);
    }
}