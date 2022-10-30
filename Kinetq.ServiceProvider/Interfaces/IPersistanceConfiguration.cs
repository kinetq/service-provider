using Microsoft.EntityFrameworkCore;

namespace Kinetq.ServiceProvider.Interfaces
{
    public interface IPersistanceConfiguration
    {
        void Configure(ModelBuilder builder);
    }
}