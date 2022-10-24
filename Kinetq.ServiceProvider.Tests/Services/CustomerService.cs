using AutoMapper;
using Kinetq.EntityFrameworkService.Builders;
using Kinetq.EntityFrameworkService.Helpers;
using Kinetq.EntityFrameworkService.Interfaces;
using Kinetq.EntityFrameworkService.Tests.Dtos;
using Kinetq.EntityFrameworkService.Tests.Models;
using Microsoft.Extensions.Logging;

namespace Kinetq.EntityFrameworkService.Tests.Services
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

        protected override IQueryable<Customer> Include(IQueryable<Customer> query)
        {
            return query;
        }

        protected override string SessionKey => "Kinetq.EntityFrameworkService.Tests";

        public IQueryable<Customer> NameEquals(IQueryable<Customer> query, Filter filter)
        {
            string value1 = filter.GetStringValue();
            return query.Where(x => x.FirstName.Equals(value1));
        }
    }
}