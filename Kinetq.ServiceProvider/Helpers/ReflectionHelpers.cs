using System.Reflection;

namespace MyPericarditis.Core.Helpers
{
    public static class ReflectionHelpers
    {
        public static IEnumerable<MethodInfo> GetMethodsBySig(this Type type, Type returnType, params Type[] parameterTypes)
        {
            return type.GetMethods().Where((m) =>
            {
                if (m.ReturnType != returnType) return false;
                var parameters = m.GetParameters();
                if ((parameterTypes == null || parameterTypes.Length == 0))
                    return parameters.Length == 0;
                if (parameters.Length != parameterTypes.Length)
                    return false;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    if (parameters[i].ParameterType != parameterTypes[i])
                        return false;
                }
                return true;
            });
        }

        public static Type[] GetTypesInNamespace(this Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes()
                .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && !t.IsEnum)
                .ToArray();
        }

        public static bool EqualsDefaultValue<T>(this T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        public static IList<Type> GetTypesThatImplInterface(this Assembly assembly, Type @interface)
        {
            var types = assembly.DefinedTypes.Where(type => type.ImplementedInterfaces.Contains(@interface)).ToList();
            types.AddRange(assembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .SelectMany(x => x.DefinedTypes)
                .Where(type => type.ImplementedInterfaces.Contains(@interface))
                .ToList());

            return types.Cast<Type>().ToList();
        }
    }
}