using AutoMapper;
using Kinetq.EntityFrameworkService.Tests.Models;

namespace Kinetq.EntityFrameworkService.Tests.Dtos
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