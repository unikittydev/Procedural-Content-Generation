using System;
using System.Collections.Generic;
using System.Reflection;

namespace PCG.Generation
{
    public static class TypeMapper
    {
        private static Type[] allTypes;
        
        private static readonly Dictionary<string, Type> typeMapper = new();

        static TypeMapper()
        {
            GetAllTypes();
        }
        
        public static Type[] GetAllTypes()
        {
            if (allTypes == null)
            {
                List<Type> types = new();
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    types.AddRange(assembly.GetTypes());

                allTypes = types.ToArray();
            }

            return allTypes;
        }
        
        public static bool AddType(Type type)
        {
            if (type.AssemblyQualifiedName == null || typeMapper.ContainsKey(type.AssemblyQualifiedName))
                return false;
            typeMapper.Add(type.AssemblyQualifiedName, type);
            return true;
        }

        public static Type GetTypeFromName(string assemblyQualifiedName)
        {
            return typeMapper[assemblyQualifiedName];
        }

        public static object CreateInstanceFromName(string assemblyQualifiedName)
        {
            if (!typeMapper.ContainsKey(assemblyQualifiedName))
                return null;
            return Activator.CreateInstance(typeMapper[assemblyQualifiedName]);
        }

        public static Type MakeGenericTypeFromParameterNames(Type type, params string[] parameterAssemblyQualifiedNames)
        {
            var parameters = new Type[parameterAssemblyQualifiedNames.Length];

            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = GetTypeFromName(parameterAssemblyQualifiedNames[i]);
            
            return type.MakeGenericType(parameters);
        }
        
        public static Type MakeGenericTypeFromNameAndGenericParameterNames(
            string typeAssemblyQualifiedName,
            params string[] parameterAssemblyQualifiedNames)
        {
            Type type = GetTypeFromName(typeAssemblyQualifiedName);
            return MakeGenericTypeFromParameterNames(type, parameterAssemblyQualifiedNames);
        }
    }
}
