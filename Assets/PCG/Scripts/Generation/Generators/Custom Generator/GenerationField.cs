using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace PCG.Generation
{
    [Serializable]
    public abstract class GenerationField
    {
        [NonSerialized]
        public FieldInfo info;

        public string fieldName;
        public bool generate;

        public string fieldTypeName;

        [SerializeReference]
        public ObjectAlternative objectAlternative;

        protected GenerationField(FieldInfo field)
        {
            info = field;
            
            fieldName = field.Name;

            fieldTypeName = field.FieldType.AssemblyQualifiedName;
        }
    }

    [Serializable]
    public abstract class GenerationField<TObj, TField> : GenerationField
    {
        protected GenerationField(FieldInfo field, bool allowManaged) : base(field)
        {
            objectAlternative = new ObjectAlternative(typeof(IGenerator<TField>), new[] { typeof(ScriptableObject) }, false, false, allowManaged);
        }
    }

    [Serializable]
    public class GenerationLeafField<TObj, TField> : GenerationField<TObj, TField>
    {
        [SerializeReference] public IGenerator<TField> generator;

        public GenerationLeafField(FieldInfo field, bool allowManaged) : base(field, allowManaged)
        {
            if (objectAlternative.choice != null)
                generator = (IGenerator<TField>)TypeMapper.CreateInstanceFromName(objectAlternative.choice);
        }
    }

    [Serializable]
    public class GenerationNestedField<TObj, TField> : GenerationField<TObj, TField>
    {
        [SerializeReference] public List<GenerationField> children = new();

        public GenerationNestedField(FieldInfo field, bool allowManaged) : base(field, allowManaged)
        {
            objectAlternative.AddChoice(typeof(GenerationNestedType));
        }
    }
}
