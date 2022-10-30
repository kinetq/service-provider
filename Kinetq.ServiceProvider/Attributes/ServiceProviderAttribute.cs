namespace Kinetq.ServiceProvider.Attributes;

public class ServiceProviderAttribute : Attribute
{
    public Type EntityType { get; set; }
}