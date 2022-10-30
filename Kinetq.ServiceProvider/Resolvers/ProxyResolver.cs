using AutoMapper;
using Kinetq.ServiceProvider.Interfaces;

namespace Kinetq.ServiceProvider.Resolvers
{
    public class ProxyResolver<T, TId> : IMemberValueResolver<object, object, TId, T> where T : class, IEntityWithTypedId<TId>
    {
        private readonly ISessionManager _sessionManager;

        public ProxyResolver(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public T Resolve(object source, object destination, TId sourceMember, T destMember, ResolutionContext context)
        {
            string sessionFactoryName = (string)context.Items["SessionKey"];
            KinetqContext dbContext = _sessionManager.GetSessionFrom(sessionFactoryName).Result;

            if (sourceMember.Equals(default(TId)))
            {
                return null;
            }

            //return the proxy from the session
            T entity = dbContext.Set<T>().Find(sourceMember);
            return entity;
        }
    }
}