using Kinetq.EntityFrameworkService.Interfaces;
using Kinetq.EntityFrameworkService.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kinetq.EntityFrameworkService.Tests.Infrastructure
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