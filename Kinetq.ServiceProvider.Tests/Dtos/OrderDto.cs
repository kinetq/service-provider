using AutoMapper;
using Kinetq.EntityFrameworkService.Tests.Models;

namespace Kinetq.EntityFrameworkService.Tests.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}