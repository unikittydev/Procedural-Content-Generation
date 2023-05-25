using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PCG.Generation
{
    /*
    public static class SerializedPropertyExtentions
    {
        private delegate FieldInfo GetFieldInfoAndStaticTypeFromProperty(SerializedProperty aProperty, out Type aType);

        private static GetFieldInfoAndStaticTypeFromProperty m_GetFieldInfoAndStaticTypeFromProperty;

        public static FieldInfo GetFieldInfoAndStaticType(this SerializedProperty prop, out Type type)
        {
            if (m_GetFieldInfoAndStaticTypeFromProperty == null)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var t in assembly.GetTypes())
                    {
                        if (t.Name == "ScriptAttributeUtility")
                        {
                            MethodInfo mi = t.GetMethod("GetFieldInfoAndStaticTypeFromProperty",
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                            m_GetFieldInfoAndStaticTypeFromProperty =
                                (GetFieldInfoAndStaticTypeFromProperty)Delegate.CreateDelegate(
                                    typeof(GetFieldInfoAndStaticTypeFromProperty), mi);
                            break;
                        }
                    }

                    if (m_GetFieldInfoAndStaticTypeFromProperty != null) break;
                }

                if (m_GetFieldInfoAndStaticTypeFromProperty == null)
                {
                    Debug.LogError("GetFieldInfoAndStaticType::Reflection failed!");
                    type = null;
                    return null;
                }
            }

            return m_GetFieldInfoAndStaticTypeFromProperty(prop, out type);
        }

        public static object GetValue(this SerializedProperty prop)
        {
            var obj = prop.serializedObject.targetObject;
            FieldInfo fieldInfo = prop.GetFieldInfoAndStaticType(out _);

            return fieldInfo.GetValue(obj);
        }

        public static void SetValue(this SerializedProperty prop, object val)
        {
            object obj = prop.serializedObject.targetObject;

            var list = new List<KeyValuePair<FieldInfo, object>>();

            foreach (var path in prop.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                FieldInfo field = type.GetField(path);
                list.Add(new KeyValuePair<FieldInfo, object>(field, obj));

                obj = field.GetValue(obj);
            }

            // Now set values of all objects, from child to parent
            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].Key.SetValue(list[i].Value, val);
                // New 'val' object will be parent of current 'val' object
                val = list[i].Value;
            }
        }

        public static T GetCustomAttributeFromProperty<T>(this SerializedProperty prop) where T : Attribute
        {
            var info = prop.GetFieldInfoAndStaticType(out _);
            return info.GetCustomAttribute<T>();
        }
    }*/
}