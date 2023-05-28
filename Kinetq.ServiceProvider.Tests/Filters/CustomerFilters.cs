using Kinetq.ServiceProvider.Attributes;
using Kinetq.ServiceProvider.Builders;
using Kinetq.ServiceProvider.Helpers;
using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Models;

namespace Kinetq.ServiceProvider.Tests.Filters;

[ServiceProvider(EntityType = typeof(Customer))]
public static class CustomerFilters
{
    public static IQueryable<Customer> NameEquals(IQueryable<Customer> query, Filter filter)
    {
        string value1 = filter.GetStringValue();
        return query.Where(x => x.FirstName.Equals(value1));
    }

    public static IQueryable<Customer> Include(IQueryable<Customer> query)
    {
        return query;
    }

    public static IEnumerable<Customer> Project(IQueryable<Customer> query, IList<Filter> filters, KinetqContext context)
    {
        return query.Select(x => new Customer()
        {
            FirstName = x.FirstName,
            LastName = x.LastName,
            Id = x.Id,
            Orders = x.Orders,
            Utilities = x.Utilities
        });
    } 
}