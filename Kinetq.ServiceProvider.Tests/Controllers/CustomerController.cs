using Kinetq.ServiceProvider.Tests.Dtos;
using Kinetq.ServiceProvider.Tests.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kinetq.ServiceProvider.Tests.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<CustomerDto> Post(CustomerDto dto)
    {
        return await _customerService.CreateAsync(dto);
    }
}