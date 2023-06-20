using Kinetq.ServiceProvider.Attributes;
using Kinetq.ServiceProvider.Tests.Models;

namespace Kinetq.ServiceProvider.Tests.Filters;

[ServiceProvider(EntityType = typeof(Order))]
public static class OrderFilters
{
    public static IQueryable<Order> Include(IQueryable<Order> query)
    {
        return query;
    }
}