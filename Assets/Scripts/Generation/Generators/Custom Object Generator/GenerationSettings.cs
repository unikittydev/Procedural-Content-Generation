using System;
using System.Reflection;

namespace PCG.Generation
{
    [Serializable]
    public class GenerationSettings<T> where T : new()
    {
        public CustomObjectNestedField<GenerationSettings<T>, T> fieldTree;

        public T _currentObject;

        public GenerationSettings()
        {
            BuildFieldTree();
        }
        
        public void BuildFieldTree()
        {
            fieldTree = new CustomObjectNestedField<GenerationSettings<T>, T>(
                typeof(GenerationSettings<T>).GetField(nameof(_currentObject),
                    BindingFlags.Instance | BindingFlags.Public));

            BuildNestedFieldChildren(fieldTree);
        }

        private void BuildNestedFieldChildren<TObj, TField>(CustomObjectNestedField<TObj, TField> parent)
        {
            FieldInfo[] fields = typeof(TField).GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
                BuildField(field, parent);
        }

        private void BuildField<TObj, TField>(FieldInfo field, CustomObjectNestedField<TObj, TField> parent)
        {
            Type nestedChildType = field.FieldType.IsPrimitive
                ? typeof(CustomObjectLeafField<,>)
                : typeof(CustomObjectNestedField<,>);
            Type nestedChildFieldType = nestedChildType.MakeGenericType(typeof(TField), field.FieldType);

            var nestedChild = Activator.CreateInstance(nestedChildFieldType, field) as CustomObjectField<TField>;
            parent.children.Add(nestedChild);

            if (field.FieldType.IsPrimitive) return;

            MethodInfo buildMethod = typeof(GenerationSettings<T>).GetMethod(nameof(BuildNestedFieldChildren),
                BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo buildMethodGeneric = buildMethod!.MakeGenericMethod(typeof(TField), field.FieldType);

            buildMethodGeneric.Invoke(this, new object[] { nestedChild });
        }
        
        public bool UpdateFieldTree()
        {
            FieldInfo field = typeof(GenerationSettings<T>).GetField(nameof(_currentObject),
                BindingFlags.Instance | BindingFlags.Public);
            return UpdateField(field, fieldTree);
        }

        private bool UpdateNestedFieldChildren<TObj, TField>(CustomObjectNestedField<TObj, TField> parent)
        {
            FieldInfo[] fields = typeof(TField).GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (parent.children.Count != fields.Length)
                return false;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo info = fields[i];
                CustomObjectField<TField> cof = parent.children[i];
                if (!UpdateField(info, cof))
                    return false;
            }

            return true;
        }

        private bool UpdateField<TField>(FieldInfo field, CustomObjectField<TField> child)
        {
            if (field.Name != child.fieldName ||
                typeof(IGenerator<>).MakeGenericType(field.FieldType).AssemblyQualifiedName != child.generatorGenericTypeName)
                return false;

            child.Update(field);
            if (field.FieldType.IsPrimitive)
                return true;

            MethodInfo buildMethod = typeof(GenerationSettings<T>).GetMethod(nameof(UpdateNestedFieldChildren),
                BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo buildMethodGeneric = buildMethod!.MakeGenericMethod(typeof(TField), field.FieldType);

            return (bool)buildMethodGeneric.Invoke(this, new object[] { child });
        }
    }
}