﻿using AutoMapper;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Dtos;
using Kinetq.ServiceProvider.Tests.Models;
using Microsoft.Extensions.Logging;

namespace Kinetq.ServiceProvider.Tests.Services
{
    public class CustomerService : KinetqServiceProvider<CustomerDto, Customer, int>, ICustomerService
    {
        public CustomerService(
            ISessionManager sessionManager, 
            ILoggerFactory loggerFactory, 
            IMapper mapper) 
            : base(sessionManager, loggerFactory, mapper)
        {
        }

        protected override string SessionKey => "Kinetq.EntityFrameworkService.Tests";
    }
}