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
        [NonSerialized]
        public FieldInfo info;

        public string fieldName;
        public bool generate;

        public string fieldTypeName;

        [SerializeReference]
        public ObjectAlternative objectAlternative;

        protected CustomField(FieldInfo field)
        {
            info = field;
            
            fieldName = field.Name;

            fieldTypeName = field.FieldType.AssemblyQualifiedName;
        }
    }

    [Serializable]
    public abstract class CustomField<TObj, TField> : CustomField<TObj>
    {
        protected CustomField(FieldInfo field) : base(field)
        {
            objectAlternative =
                new ObjectAlternative(typeof(IGenerator<TField>), new[] { typeof(ScriptableObject) }, false, false);
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
    }

    [Serializable]
    public class CustomNestedField<TObj, TField> : CustomField<TObj, TField>
    {
        [SerializeReference] public List<CustomField<TField>> children = new();

        public CustomNestedField(FieldInfo field) : base(field)
        {
            objectAlternative.AddChoice(typeof(CustomObjectGeneratorNestedType));
        }
    }
}
