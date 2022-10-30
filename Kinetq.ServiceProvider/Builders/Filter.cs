namespace Kinetq.ServiceProvider.Builders
{
    public class Filter
    {
        public string Name { get; set; }
        public dynamic Value { get; set; }

        public static Filter Of(string name, dynamic value)
        {
            return new Filter { Name = name, Value = value };
        }
    }
}