using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Type = System.Type;

namespace PCG.Generation
{
    [InitializeOnLoad]
    public static class ObjectMapper
    {
        /// <summary>
        /// Contains key-value type pairs of the form:
        /// key:   T&lt;&gt;
        /// value: T&lt;U&gt;
        /// </summary>
        private static readonly Dictionary<Type, List<Type>> objectSubClassesByGenericBaseType = new();
        
        /// <summary>
        /// Contains key-value type pairs of the form:
        /// key:   T&lt;U&gt;
        /// value: V
        /// where V : T&lt;U&gt;
        /// </summary>
        private static readonly Dictionary<Type, List<Type>> objectSubClassesByParamGenericBaseType = new();

        /// <summary>
        /// Contains key-value pairs of the types and their display names
        /// </summary>
        private static readonly Dictionary<Type, string> objectTypeToName = new();

        /// <summary>
        /// Generic base types which were allowed when registering subclasses
        /// </summary>
        private static readonly List<Type> allowedGenericBaseTypes = new();

        private static readonly Dictionary<Type, Func<object>> objectCreators = new();

        public static void RegisterSubClasses(Type genericBase, Type[] excludedBaseTypes, bool allowGenerics, bool createConstructors)
        {
            if (allowGenerics)
                allowedGenericBaseTypes.Add(genericBase);
            
            foreach (Type type in TypeMapper.GetAllTypes())
            {
                // Avoid self
                if (genericBase == type)
                    continue;
                // Avoid generics if banned
                if (!allowGenerics && type.IsGenericType) 
                    continue;
                // Avoid excluded types
                if (excludedBaseTypes != null && excludedBaseTypes.Any(excludedType => excludedType.IsAssignableFrom(type)))
                    continue;
                // Check if class is assignable
                if (type.IsAssignableToGenericType(genericBase, out Type paramGenericBase))
                    AddSubClass(type, genericBase, paramGenericBase, createConstructors);
            }
        }

        public static void RegisterParamGenericSubClasses(Type genericParameter, Type[] excludedBaseTypes, Type[] additionalGenericParameters)
        {
            Type[] GetGenericParameters()
            {
                if (additionalGenericParameters == null)
                    return new[] { genericParameter };
                var parameters = new Type[additionalGenericParameters.Length + 1];
                Array.Copy(additionalGenericParameters, 0, parameters, 1, additionalGenericParameters.Length);
                return parameters;
            }
            
            // Look for each allowed generic of type T<>
            // For example, IObjectProvider<>
            foreach (Type genericBase in allowedGenericBaseTypes)
            {
                if (excludedBaseTypes != null && excludedBaseTypes.Contains(genericBase))
                    continue;

                // Make parametrized generic of type T<V>
                var paramGenericBase = genericBase.MakeGenericType(GetGenericParameters());
                objectSubClassesByParamGenericBaseType.Add(paramGenericBase, new List<Type>());

                TypeMapper.AddType(paramGenericBase);
                
                // Get all subclasses of T<>
                // For example, MonoObjectProvider<>
                Type[] genericSubTypes = objectSubClassesByGenericBaseType[genericBase]
                    .SelectMany(genericType => objectSubClassesByParamGenericBaseType[genericType]).ToArray();
                foreach (Type genericSub in genericSubTypes)
                {
                    // Make parametrized generic with the same parameters
                    try
                    {
                        var paramGenericSub = genericSub.MakeGenericType(GetGenericParameters());
                        AddSubClass(paramGenericSub, genericBase, paramGenericBase, false);
                    }
                    catch { /**/ }
                }
            }
        }
        
        public static void AddSubClass(Type sub, Type genericBase, Type paramGenericBase, bool createConstructor)
        {
            // Registering subclass
            if (!objectSubClassesByGenericBaseType.ContainsKey(genericBase))
            {
                objectSubClassesByGenericBaseType.Add(genericBase, new List<Type>());
                TypeMapper.AddType(genericBase);
            }
    
            objectSubClassesByGenericBaseType[genericBase].Add(paramGenericBase);

            if (!objectSubClassesByParamGenericBaseType.ContainsKey(paramGenericBase))
            {
                objectSubClassesByParamGenericBaseType.Add(paramGenericBase, new List<Type>());
                TypeMapper.AddType(paramGenericBase);
            }
            
            if (genericBase != paramGenericBase)
                objectSubClassesByParamGenericBaseType[paramGenericBase].Add(sub);
            TypeMapper.AddType(sub);

            // Setting it's name
            string name = sub.Name;
            foreach (Attribute attribute in sub.GetCustomAttributes())
                if (attribute is InspectorNameAttribute nameAttribute)
                {
                    name = nameAttribute.displayName;
                    break;
                }
            
            objectTypeToName.TryAdd(sub, name);
            
            // Setting it's constructor if needed
            if (createConstructor)
                objectCreators.Add(sub, () => Activator.CreateInstance(sub));
        }
        
        public static object CreateObject(Type type)
        {
            return objectCreators[type]();
        }
        
        public static object CreateObject(string typeName)
        {
            return objectCreators[TypeMapper.GetTypeFromName(typeName)]();
        }
        
        public static List<Type> GetObjectTypes(Type paramGenericType)
        {
            if (!objectSubClassesByParamGenericBaseType.ContainsKey(paramGenericType))
                return null;
            return objectSubClassesByParamGenericBaseType[paramGenericType];
        }

        public static List<Type> GetObjectTypes(string paramGenericTypeName)
        {
            Type type = TypeMapper.GetTypeFromName(paramGenericTypeName);
            return GetObjectTypes(type);
        }

        public static bool GetObjectTypes(Type paramGenericType, out List<Type> types)
        {
            types = GetObjectTypes(paramGenericType);
            return types != null;
        }
        
        public static bool GetObjectTypes(string paramGenericTypeName, out List<Type> types)
        {
            types = GetObjectTypes(paramGenericTypeName);
            return types != null;
        }

        public static List<string> GetObjectDisplayNames(Type paramGenericType)
        {
            List<Type> types = GetObjectTypes(paramGenericType);
            List<string> names = types.Select(type => objectTypeToName[type]).ToList();
            return names;
        }

        public static List<string> GetObjectDisplayNames(string paramGenericTypeName)
        {
            Type type = TypeMapper.GetTypeFromName(paramGenericTypeName);
            return GetObjectDisplayNames(type);
        }
        
        public static Type GetTypeFromDisplayName(Type returnType, string objectName)
        {
            List<string> names = GetObjectDisplayNames(returnType);
            int index = names.FindIndex(item => string.Equals(item, objectName));
            List<Type> types = GetObjectTypes(returnType);
            return types[index];
        }

        public static Type GetTypeFromDisplayName(string returnTypeName, string objectName)
        {
            var type = TypeMapper.GetTypeFromName(returnTypeName);
            return GetTypeFromDisplayName(type, objectName);
        }
        
        /// <summary>
        /// Returns true if sub is directly or indirectly implements super 
        /// </summary>
        /// <param name="sub">Child type</param>
        /// <param name="genericBase">Parent type</param>
        /// <param name="paramGenericBase">super type with generic parameters</param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(this Type sub, Type genericBase, out Type paramGenericBase)
        {
            while (true)
            {
                // super is T<> and sub itself is T<U>
                if (sub.IsGenericType && sub.GetGenericTypeDefinition() == genericBase)
                {
                    paramGenericBase = sub;
                    return true;
                }
                
                Type[] interfaces = sub.GetInterfaces();

                // super is T<> and sub has interface of T<U>
                foreach (Type @interface in interfaces)
                    if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == genericBase)
                    {
                        paramGenericBase = @interface;
                        return true;
                    }

                // Go one level deeper and start again
                Type baseType = sub.BaseType;
                if (baseType != null)
                {
                    sub = baseType;
                    continue;
                }

                // Haven't found more base types, so type is not assignable
                paramGenericBase = null;
                return false;
            }
        }
    }
}
