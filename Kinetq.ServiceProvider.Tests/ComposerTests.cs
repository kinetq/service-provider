using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Composers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kinetq.ServiceProvider.Tests;

public class ComposerTests : ATests
{
    [Fact]
    public async Task Can_Compose_With_Composer()
    {
        await AddCustomer();
        var composer = ServiceProvider.GetService<IKinetqComposer>();

        var result =
            await composer.ComposeWith<CustomerCountComposer>()
                .Execute();

        Assert.Equal(1, result);
    }
}