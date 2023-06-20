using Kinetq.ServiceProvider.Config;
using Kinetq.ServiceProvider.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kinetq.ServiceProvider.Managers
{
    public class SessionManager : ISessionManager
    {
        private readonly string SESSIONS_KEY = "DBCONTEXT_CONTEXT_SESSIONS";

        private readonly IConfigurationManager<EFOptions> _configurationManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDictionary<string, KinetqContext> _backupContextSessions = new Dictionary<string, KinetqContext>();
        private readonly IEnumerable<IPersistanceConfiguration> _persistanceConfigurations;

        public SessionManager(
            IConfigurationManager<EFOptions> configurationManager,
            IHttpContextAccessor httpContextAccessor, 
            IEnumerable<IPersistanceConfiguration> persistanceConfigurations)
        {
            _configurationManager = configurationManager;
            _httpContextAccessor = httpContextAccessor;
            _persistanceConfigurations = persistanceConfigurations;
        }

        public KinetqContext GetSessionFrom(string sessionKey)
        {
            if (!ContextSessions.TryGetValue(sessionKey, out var context))
            {
                var config = 
                    _configurationManager.Configurations.First(x => x.SessionKey.Equals(sessionKey));

                context = new KinetqContext(config.Options, _persistanceConfigurations);
                ContextSessions.Add(sessionKey, context);
            }

            return context;
        }

        public async Task<KinetqContext> GetTransientSession(string sessionKey)
        {
            var config = _configurationManager.Configurations.First(x => x.SessionKey.Equals(sessionKey));
            return new KinetqContext(config.Options, _persistanceConfigurations);
        }

        public void CloseSessionOn(string sessionKey)
        {
            if (ContextSessions.TryGetValue(sessionKey, out var context))
            {
                context.Dispose();
                ContextSessions.Remove(sessionKey);
            }
        }

        public IDictionary<string, KinetqContext> ContextSessions
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                {
                    return _backupContextSessions;
                }

                if (!_httpContextAccessor.HttpContext.Items.TryGetValue(SESSIONS_KEY, out var sessions))
                {
                    sessions = new Dictionary<string, KinetqContext>();
                    _httpContextAccessor.HttpContext.Items.Add(SESSIONS_KEY, sessions);
                }

                return (IDictionary<string, KinetqContext>)sessions;
            }
        }
    }
}