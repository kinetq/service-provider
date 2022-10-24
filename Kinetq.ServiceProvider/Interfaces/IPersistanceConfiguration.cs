using Microsoft.EntityFrameworkCore;

namespace Kinetq.EntityFrameworkService.Interfaces
{
    public interface IPersistanceConfiguration
    {
        void Configure(ModelBuilder builder);
    }
}