using System;
using System.Reflection;

namespace PCG.Generation
{    
    [Serializable]
    public class GenerationSettings<T> where T : new()
    {
        public GenerationNestedField<GenerationSettings<T>, T> fieldTree;

        public T currentObject;

        public GenerationSettings(bool allowManaged)
        {
            BuildFieldTree(allowManaged);
        }
        
        public void BuildFieldTree(bool allowManaged)
        {
            fieldTree = new GenerationNestedField<GenerationSettings<T>, T>(
                typeof(GenerationSettings<T>).GetField(nameof(currentObject),
                    BindingFlags.Instance | BindingFlags.Public), allowManaged) { generate = true };

            BuildNestedFieldChildren(fieldTree, allowManaged);
        }

        private void BuildNestedFieldChildren<TObj, TField>(GenerationNestedField<TObj, TField> parent, bool allowManaged)
        {
            FieldInfo[] fields = typeof(TField).GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
                BuildField(field, parent, allowManaged);
        }

        private void BuildField<TObj, TField>(FieldInfo field, GenerationNestedField<TObj, TField> parent, bool allowManaged)
        {
            Type nestedChildType = field.FieldType.IsPrimitive
                ? typeof(GenerationLeafField<,>)
                : typeof(GenerationNestedField<,>);
            Type nestedChildFieldType = nestedChildType.MakeGenericType(typeof(TField), field.FieldType);

            var nestedChild = (GenerationField)Activator.CreateInstance(nestedChildFieldType, field, allowManaged);
            parent.children.Add(nestedChild);

            if (field.FieldType.IsPrimitive) return;

            MethodInfo buildMethod = typeof(GenerationSettings<T>).GetMethod(nameof(BuildNestedFieldChildren),
                BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo buildMethodGeneric = buildMethod!.MakeGenericMethod(typeof(TField), field.FieldType);

            buildMethodGeneric.Invoke(this, new object[] { nestedChild, allowManaged });
        }
        
        public bool UpdateFieldTree()
        {
            FieldInfo field = typeof(GenerationSettings<T>).GetField(nameof(currentObject),
                BindingFlags.Instance | BindingFlags.Public);
            return UpdateField<GenerationSettings<T>>(field, fieldTree);
        }

        private bool UpdateNestedFieldChildren<TObj, TField>(GenerationNestedField<TObj, TField> parent)
        {
            FieldInfo[] fields = typeof(TField).GetFields(BindingFlags.Public | BindingFlags.Instance);

            if (parent.children.Count != fields.Length)
                return false;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo info = fields[i];
                GenerationField cof = parent.children[i];
                if (!UpdateField<TField>(info, cof))
                    return false;
            }

            return true;
        }

        private bool UpdateField<TField>(FieldInfo field, GenerationField child)
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