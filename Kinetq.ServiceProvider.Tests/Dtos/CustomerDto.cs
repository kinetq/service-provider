using Kinetq.ServiceProvider.Tests.Models;

namespace Kinetq.ServiceProvider.Tests.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Utilities Utilities { get; set; }
        public IEnumerable<int> OrderIds { get; set; }
    }
}