using Kinetq.EntityFrameworkService.Interfaces;

namespace Kinetq.EntityFrameworkService.Tests.Models
{
    public class Order : IEntity<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}