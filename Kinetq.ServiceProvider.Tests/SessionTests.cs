using Kinetq.ServiceProvider.Interfaces;
using Kinetq.ServiceProvider.Tests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kinetq.ServiceProvider.Tests
{
    public class SessionTests : ATests
    {
        [Fact]
        public async Task Test_Add_Entity()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = session.GetSessionFrom(SessionKey);

            var dbSet = context.Set<Customer>();
            dbSet.Add(new Customer { FirstName = "Sam", LastName = "Sinno", Id = 1 });
            context.SaveChanges();
            session.CloseSessionOn(SessionKey);

            var context1 = session.GetSessionFrom(SessionKey);
            var dbSet1 = context1.Set<Customer>();

            var customer = dbSet1.Find(1);
            Assert.Equal(customer.FirstName, "Sam");
        }

        [Fact]
        public async Task Test_Update_Entity()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = session.GetSessionFrom(SessionKey);

            var dbSet = context.Set<Customer>();
            dbSet.Add(new Customer { FirstName = "Sam", LastName = "Sinno", Id = 1 });
            context.SaveChanges();
            session.CloseSessionOn(SessionKey);

            var context1 = session.GetSessionFrom(SessionKey);
            var dbSet1 = context1.Set<Customer>();

            var customer = dbSet1.Find(1);
            customer.FirstName = "Tom";
            var entry = context1.Entry(customer);
            Assert.Equal(EntityState.Modified, entry.State);

            dbSet1.Update(customer);
            context1.SaveChanges();

            var customer1 = dbSet1.Find(1);
            Assert.Equal(customer1.FirstName, "Tom");
        }

        [Fact]
        public async Task Test_Multiple_Enum()
        {
            var session = ServiceProvider.GetService<ISessionManager>();
            var context = session.GetSessionFrom(SessionKey);

            var dbSet = context.Set<Customer>();
            var newCustomer = new Customer
            {
                FirstName = "Sam",
                LastName = "Sinno",
                Id = 1,
                Utilities = Utilities.CableInternet | Utilities.Electricity
            };

            dbSet.Add(newCustomer);

            context.SaveChanges();
            session.CloseSessionOn(SessionKey);

            var context1 = session.GetSessionFrom(SessionKey);
            var dbSet1 = context1.Set<Customer>();

            var customer = dbSet1.Find(1);
            var utilitiesToMatch = Utilities.CableInternet | Utilities.Electricity;
            Assert.True(customer.Utilities.HasFlag(utilitiesToMatch));
        }
    }
}