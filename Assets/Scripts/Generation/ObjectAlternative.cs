using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PCG.Generation
{
    [Serializable]
    public class ObjectAlternative
    {
        public string choice;
        
        public List<string> choices = new();
        public List<string> choiceNames = new();

        public ObjectAlternative(Type baseParamType, Type[] excludedBaseTypes, bool allowSelf, bool allowGenerics)
        {
            var types = TypeMapper.GetAllTypes();

            var baseType = baseParamType.IsGenericType ? baseParamType.GetGenericTypeDefinition() : baseParamType;
            foreach (var type in types)
            {
                // Avoid self
                if (!allowSelf && baseType == type)
                    continue;
                // Avoid generics if banned
                if (!allowGenerics && type.IsGenericType) 
                    continue;
                // Avoid excluded types
                if (excludedBaseTypes != null && excludedBaseTypes.Any(excludedType => excludedType.IsAssignableFrom(type)))
                    continue;
                // Check if class is assignable
                if (!IsAssignedFromGenericType(type, baseParamType, out Type paramSubType))
                    continue;
                
                AddChoice(paramSubType);
            }
        }
        
        public ObjectAlternative(Type baseParamType, Type[] excludedBaseTypes, bool allowGenerics) : this(baseParamType, excludedBaseTypes, false, allowGenerics) { }
            
        public ObjectAlternative(Type baseParamType, bool allowSelf, bool allowGenerics) : this(baseParamType, null, allowSelf, allowGenerics) { }
        
        public ObjectAlternative(Type baseParamType, bool allowGenerics) : this(baseParamType, null, false, allowGenerics) { }

        public void AddChoice(Type type)
        {
            var choiceTypeName = type.AssemblyQualifiedName;

            var inspectorName = type.GetCustomAttributes(typeof(InspectorNameAttribute), false).FirstOrDefault() as InspectorNameAttribute;
            var choiceDisplayName = inspectorName?.displayName ?? type.Name;
            
            choices.Add(choiceTypeName);
            choiceNames.Add(choiceDisplayName);

            TypeMapper.AddType(type);

            choice ??= choiceTypeName;
        }
        
        public string GetCurrentName()
        {
            int index = choices.IndexOf(choice);
            
            var name = index >= 0 ? choiceNames[index] : null;
            return name;
        }

        public void SetValueFromName(string newName)
        {
            int index = choiceNames.IndexOf(newName);
            choice = choices[index];
        }
        
        // true (ReferenceObjectProvider<>, IObjectProvider<T>, out ReferenceObjectProvider<T>)
        // true (UniformFloatGenerator, IGenerator<float>, out UniformFloatGenerator)
        private static bool IsAssignedFromGenericType(Type subType, Type paramBaseType, out Type paramSubType)
        {
            try
            {
                // decompose T<U0, ..., Un> into T<> and [U0, ..., Un]
                Type baseType = paramBaseType.GetGenericTypeDefinition();
                Type[] baseParams = paramBaseType.GetGenericArguments();

                paramSubType = subType.IsGenericType ? subType.MakeGenericType(baseParams) : subType;
                
                while (true)
                {
                    // base is T<> and sub is T<>
                    if (subType == baseType)
                        return true;

                    Type[] interfaces = paramSubType.GetInterfaces();

                    // base is T<U> and sub has interface of T<U>
                    foreach (Type @interface in interfaces)
                        if (@interface == paramBaseType)
                            return true;

                    // get base of sub and start again
                    var subParentType = subType.BaseType;
                    if (subParentType != null)
                    {
                        subType = subParentType.IsGenericType
                            ? subParentType.GetGenericTypeDefinition()
                            : subParentType;
                        continue;
                    }

                    paramSubType = null;
                    return false;
                }
            }
            catch
            {
                paramSubType = null;
                return false;
            }
        }
    }
}
