namespace Kinetq.ServiceProvider.Interfaces
{
    public interface ISessionManager
    {
        Task<KinetqContext> GetSessionFrom(string sessionKey);
        Task<KinetqContext> GetTransientSession(string sessionKey);
        void CloseSessionOn(string sessionKey);
        IDictionary<string, KinetqContext> ContextSessions { get; }
    }
}