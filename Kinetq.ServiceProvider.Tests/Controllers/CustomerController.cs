using Kinetq.EntityFrameworkService.Tests.Dtos;
using Kinetq.EntityFrameworkService.Tests.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kinetq.EntityFrameworkService.Tests.Controllers;

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