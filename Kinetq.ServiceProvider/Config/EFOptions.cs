using Microsoft.EntityFrameworkCore;

namespace Kinetq.ServiceProvider.Config
{
    public class EFOptions
    {
        public DbContextOptions Options { get; set; }
        public string SessionKey { get; set; }
        public Type ContextType { get; set; }
    }
}