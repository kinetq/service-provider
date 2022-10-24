using Kinetq.EntityFrameworkService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kinetq.EntityFrameworkService;

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