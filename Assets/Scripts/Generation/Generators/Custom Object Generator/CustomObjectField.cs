using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [Serializable]
    public abstract class CustomObjectField<TObj>
    {
        public string fieldName;
        public bool generate;

        public string generatorGenericTypeName;

        [SerializeReference]
        public ObjectAlternative objectAlternative;
        
        public abstract void Update(FieldInfo field);

        public abstract TObj GenerateField(TObj target, ref Random random);
    }

    [Serializable]
    public abstract class CustomObjectField<TObj, TField> : CustomObjectField<TObj>
    {
        [NonSerialized] public ExpressionGetterDelegate<TObj, TField> getter;
        [NonSerialized] public ExpressionSetterDelegate<TObj, TField> setter;

        protected CustomObjectField(FieldInfo field)
        {
            fieldName = field.Name;

            getter = CustomObjectFieldMapper<TObj, TField>.GetGetter(field);
            setter = CustomObjectFieldMapper<TObj, TField>.GetSetter(field);

            generatorGenericTypeName = typeof(IGenerator<TField>).AssemblyQualifiedName;

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
    public class CustomObjectLeafField<TObj, TField> : CustomObjectField<TObj, TField>
    {
        [SerializeReference] private IGenerator<TField> generator;

        public CustomObjectLeafField(FieldInfo field) : base(field)
        {
            if (objectAlternative.choice != null)
                generator = (IGenerator<TField>)TypeMapper.CreateInstanceFromName(objectAlternative.choice);
        }

        public override TObj GenerateField(TObj target, ref Random random)
        {
            var value = generator.Generate(ref random);
            setter(ref target, value);
            return target;
        }
    }

    [Serializable]
    public class CustomObjectNestedField<TObj, TField> : CustomObjectField<TObj, TField>
    {
        [SerializeReference] public List<CustomObjectField<TField>> children = new();

        public CustomObjectNestedField(FieldInfo field) : base(field)
        {
            objectAlternative.AddChoice(typeof(CustomObjectGeneratorNestedType));
        }

        public override TObj GenerateField(TObj target, ref Random random)
        {
            TField nested = getter(target);

            foreach (CustomObjectField<TField> child in children)
                if (child.generate)
                    nested = child.GenerateField(nested, ref random);

            setter(ref target, nested);

            return target;
        }
    }
}
