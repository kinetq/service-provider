namespace Kinetq.ServiceProvider.Interfaces
{
    public interface IConfigurationManager<TArgs>
    {
        List<TArgs> Configurations { get; }
        void AddConfiguration(TArgs config);
    }
}