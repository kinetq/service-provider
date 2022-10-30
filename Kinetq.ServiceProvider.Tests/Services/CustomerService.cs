using AutoMapper;
using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Helpers;
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

        public IQueryable<Customer> NameEquals(IQueryable<Customer> query, Filter filter)
        {
            string value1 = filter.GetStringValue();
            return query.Where(x => x.FirstName.Equals(value1));
        }
    }
}