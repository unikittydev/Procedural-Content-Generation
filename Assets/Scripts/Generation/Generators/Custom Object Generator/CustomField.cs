using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [Serializable]
    public abstract class CustomField<TObj>
    {
        public string fieldName;
        public bool generate;

        public string fieldTypeName;

        [SerializeReference]
        public ObjectAlternative objectAlternative;

        protected CustomField(FieldInfo field)
        {
            fieldName = field.Name;

            fieldTypeName = field.FieldType.AssemblyQualifiedName;
        }
        
        public abstract void Update(FieldInfo field);

        public abstract void GenerateField(ref TObj target, ref Random random);
    }

    [Serializable]
    public abstract class CustomField<TObj, TField> : CustomField<TObj>
    {
        public ExpressionGetterDelegate<TObj, TField> getter;
        public ExpressionSetterDelegate<TObj, TField> setter;

        protected CustomField(FieldInfo field) : base(field)
        {
            getter = CustomObjectFieldMapper<TObj, TField>.GetGetter(field);
            setter = CustomObjectFieldMapper<TObj, TField>.GetSetter(field);

            objectAlternative =
                new ObjectAlternative(typeof(IGenerator<TField>), new[] { typeof(ScriptableObject) }, false, false);
        }

        public override void Update(FieldInfo field)
        {
            getter = CustomObjectFieldMapper<TObj, TField>.GetGetter(field);
            setter = CustomObjectFieldMapper<TObj, TField>.GetSetter(field);
        }
    }

    [Serializable]
    public class CustomLeafField<TObj, TField> : CustomField<TObj, TField>
    {
        [SerializeReference] public IGenerator<TField> generator;

        public CustomLeafField(FieldInfo field) : base(field)
        {
            if (objectAlternative.choice != null)
                generator = (IGenerator<TField>)TypeMapper.CreateInstanceFromName(objectAlternative.choice);
        }

        public override void GenerateField(ref TObj target, ref Random random)
        {
            TField value = generator.Generate(ref random);
            setter(ref target, value);
        }
    }

    [Serializable]
    public class CustomNestedField<TObj, TField> : CustomField<TObj, TField>
    {
        [SerializeReference] public List<CustomField<TField>> children = new();

        public CustomNestedField(FieldInfo field) : base(field)
        {
            objectAlternative.AddChoice(typeof(CustomObjectGeneratorNestedType));
        }

        public override void GenerateField(ref TObj target, ref Random random)
        {
            TField nested = getter(target);

            foreach (CustomField<TField> child in children)
                if (child.generate)
                    child.GenerateField(ref nested, ref random);
            
            setter(ref target, nested);
        }
    }
}
