using System.Text.Json;
using Kinetq.ServiceProvider.Builders;

namespace Kinetq.ServiceProvider.Helpers
{
    public static class FilterHelpers
    {
        public static int GetInt32Value(this Filter filter)
        {
            if (filter.Value is JsonElement)
            {
                return ((JsonElement)filter.Value).GetInt32();
            }

            return (int)filter.Value;
        } 
        
        public static bool GetBooleanValue(this Filter filter)
        {
            if (filter.Value is JsonElement)
            {
                return ((JsonElement)filter.Value).GetBoolean();
            }

            return (bool)filter.Value;
        }   
        
        public static bool[] GetBooleanValues(this IList<Filter> filters, string name)
        {
            return filters
                .Where(x => x.Name.Equals(name))
                .Select(x => x.GetBooleanValue()).ToArray();
        }
        
        public static string GetStringValue(this Filter filter)
        {
            if (filter.Value is JsonElement)
            {
                return ((JsonElement)filter.Value).GetString();
            }

            return (string)filter.Value;
        }

        public static string[] GetStringValues(this IList<Filter> filters, string name)
        {
            return filters
                .Where(x => x.Name.Equals(name))
                .Select(x => x.GetStringValue()).ToArray();
        }
        
        public static int[] GetIntValues(this IList<Filter> filters, string name)
        {
            return filters
                .Where(x => x.Name.Equals(name))
                .Select(x => x.GetInt32Value())
                .ToArray();
        }
    }
}