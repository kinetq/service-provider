using Kinetq.EntityFrameworkService.Config;
using Kinetq.EntityFrameworkService.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Kinetq.EntityFrameworkService.Middleware
{
    public class CloseSessionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISessionManager _sessionManager;
        private readonly IConfigurationManager<EFOptions> _configurationManager;

        public CloseSessionMiddleware(RequestDelegate next, ISessionManager sessionManager, IConfigurationManager<EFOptions> configurationManager)
        {
            _next = next;
            _sessionManager = sessionManager;
            _configurationManager = configurationManager;
        }

        public Task Invoke(HttpContext context) => InvokeAsync(context); // Stops VS from nagging about async method without ...Async suffix.

        async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                foreach (var config in _configurationManager.Configurations)
                {
                    _sessionManager.CloseSessionOn(config.SessionKey);
                }
            }
        }
    }
}