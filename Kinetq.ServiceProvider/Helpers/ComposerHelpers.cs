using System.Reflection;
using Kinetq.ServiceProvider.Models;

namespace Kinetq.ServiceProvider.Helpers;

public static class ComposerHelpers
{
    public static Task<T> GetResult<T>(this IList<ServiceResult> results)
    {
        var result =  results.First(x => x.ReturnType == typeof(T));
        if (result.HasRun)
        {
            return (Task<T>)result.Value;
        }

        result.Unwrap();

        return (Task<T>)result.Value;
    }
}