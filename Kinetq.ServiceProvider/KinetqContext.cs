using Kinetq.ServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kinetq.ServiceProvider;

public class KinetqContext : DbContext
{
    private readonly IEnumerable<IPersistanceConfiguration> _persistanceConfigurations;
    // For testing purposes
    public KinetqContext()
    {

    }
    public KinetqContext(DbContextOptions options, IEnumerable<IPersistanceConfiguration> persistanceConfigurations) : base(options)
    {
        _persistanceConfigurations = persistanceConfigurations;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var persistenceConfiguration in _persistanceConfigurations)
        {
            persistenceConfiguration.Configure(modelBuilder);
        }
    }
}