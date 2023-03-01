using System;
using System.Reflection;

namespace PCG.Generation
{
    [Serializable]
    public class GenerationSettings<T> where T : new()
    {
        public CustomNestedField<GenerationSettings<T>, T> fieldTree;

        public T currentObject;

        public GenerationSettings()
        {
            BuildFieldTree();
        }
        
        public void BuildFieldTree()
        {
            fieldTree = new CustomNestedField<GenerationSettings<T>, T>(
                typeof(GenerationSettings<T>).GetField(nameof(currentObject),
                    BindingFlags.Instance | BindingFlags.Public)) { generate = true };

            BuildNestedFieldChildren(fieldTree);
        }

        private void BuildNestedFieldChildren<TObj, TField>(CustomNestedField<TObj, TField> parent)
        {
            FieldInfo[] fields = typeof(TField).GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
                BuildField(field, parent);
        }

        private void BuildField<TObj, TField>(FieldInfo field, CustomNestedField<TObj, TField> parent)
        {
            Type nestedChildType = field.FieldType.IsPrimitive
                ? typeof(CustomLeafField<,>)
                : typeof(CustomNestedField<,>);
            Type nestedChildFieldType = nestedChildType.MakeGenericType(typeof(TField), field.FieldType);

            var nestedChild = (CustomField)Activator.CreateInstance(nestedChildFieldType, field);
            parent.children.Add(nestedChild);

            if (field.FieldType.IsPrimitive) return;

            MethodInfo buildMethod = typeof(GenerationSettings<T>).GetMethod(nameof(BuildNestedFieldChildren),
                BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo buildMethodGeneric = buildMethod!.MakeGenericMethod(typeof(TField), field.FieldType);

            buildMethodGeneric.Invoke(this, new object[] { nestedChild });
        }
        
        public bool UpdateFieldTree()
        {
            FieldInfo field = typeof(GenerationSettings<T>).GetField(nameof(currentObject),
                BindingFlags.Instance | BindingFlags.Public);
            return UpdateField<GenerationSettings<T>>(field, fieldTree);
        }

        private bool UpdateNestedFieldChildren<TObj, TField>(CustomNestedField<TObj, TField> parent)
        {
            FieldInfo[] fields = typeof(TField).GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (parent.children.Count != fields.Length)
                return false;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo info = fields[i];
                CustomField cof = parent.children[i];
                if (!UpdateField<TField>(info, cof))
                    return false;
            }

            return true;
        }

        private bool UpdateField<TField>(FieldInfo field, CustomField child)
        {
            if (field.Name != child.fieldName ||
                field.FieldType.AssemblyQualifiedName != child.fieldTypeName)
                return false;

            child.info = field;
            
            if (field.FieldType.IsPrimitive)
                return true;

            MethodInfo buildMethod = typeof(GenerationSettings<T>).GetMethod(nameof(UpdateNestedFieldChildren),
                BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo buildMethodGeneric = buildMethod!.MakeGenericMethod(typeof(TField), field.FieldType);

            return (bool)buildMethodGeneric.Invoke(this, new object[] { child });
        }
    }
}