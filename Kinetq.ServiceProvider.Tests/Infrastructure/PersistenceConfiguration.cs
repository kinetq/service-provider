using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Models;
using Microsoft.EntityFrameworkCore;

namespace Kinetq.ServiceProvider.Tests.Infrastructure
{
    public class PersistenceConfiguration : IPersistanceConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<Customer>()
                .HasMany<Order>()
                .WithOne()
                .IsRequired();

            builder.Entity<Order>();
        }
    }
}