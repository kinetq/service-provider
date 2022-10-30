using Kinetq.ServiceProvider.Interfaces;

namespace Kinetq.ServiceProvider.Tests.Models
{
    public class Customer : IEntityWithTypedId<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Utilities Utilities { get; set; }
        public IList<Order> Orders { get; set; }
        public int Id { get; set; }
    }
}