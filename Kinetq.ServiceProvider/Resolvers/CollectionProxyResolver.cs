using AutoMapper;
using Kinetq.ServiceProvider.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kinetq.ServiceProvider.Resolvers
{
    public class CollectionProxyResolver<T, TId> : IMemberValueResolver<object, object, IEnumerable<TId>, IList<T>> 
        where T : class, IEntityWithTypedId<TId>
    {
        private readonly ISessionManager _sessionManager;

        public CollectionProxyResolver(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public IList<T> Resolve(object source, object destination, IEnumerable<TId> sourceMember, IList<T> destMember, ResolutionContext context)
        {
            List<T> entityList = new List<T>();
            if (sourceMember == null || !sourceMember.Any())
            {
                return entityList;
            }

            string sessionKey = (string)context.Items["SessionKey"];
            KinetqContext dbContext = _sessionManager.GetSessionFrom(sessionKey).Result;

            DbSet<T> set = dbContext.Set<T>();
            entityList.AddRange(set.Where(x => sourceMember.Contains(x.Id)));

            return entityList;
        }
    }
}