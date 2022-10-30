using Kinetq.ServiceProvider.Config;
using Kinetq.ServiceProvider.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kinetq.ServiceProvider.Managers
{
    public class ConfigurationManager : IConfigurationManager<EFOptions>
    {
        private readonly Lazy<List<EFOptions>> _configurations =
            new Lazy<List<EFOptions>>(() => new List<EFOptions>());

        private readonly ILoggerFactory _loggerFactory;

        public ConfigurationManager(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        private ILogger Logger => CreateLogger<ConfigurationManager>();

        private ILogger CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger<ConfigurationManager>();
        }

        public List<EFOptions> Configurations => _configurations.Value;

        public void AddConfiguration(EFOptions config)
        {
            if (Configurations.Any(x => x.SessionKey == config.SessionKey))
            {
                return;
            }

            Configurations.Add(config);
        }
    }
}