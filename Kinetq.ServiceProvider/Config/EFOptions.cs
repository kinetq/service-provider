using Microsoft.EntityFrameworkCore;

namespace Kinetq.EntityFrameworkService.Config
{
    public class EFOptions
    {
        public DbContextOptions Options { get; set; }
        public string SessionKey { get; set; }
        public Type ContextType { get; set; }
    }
}