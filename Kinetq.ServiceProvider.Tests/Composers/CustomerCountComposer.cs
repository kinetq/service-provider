using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Services;

namespace Kinetq.ServiceProvider.Tests.Composers;

public class CustomerCountComposer : ComposerBuilder<int>, IComposer
{
    private readonly ICustomerService _customerService;
    public CustomerCountComposer(ISessionManager sessionManager, ICustomerService customerService) 
        : base( sessionManager)
    {
        _customerService = customerService;
    }

    protected override async Task<int> Arrange()
    {
        return await _customerService.GetCountAsync();
    }
}