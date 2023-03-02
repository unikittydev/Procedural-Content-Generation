using System;
using System.Collections.Generic;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public abstract class ObjectFieldGenerator<TObj>
    {
        public abstract void GenerateField(ref TObj target, ref Random random);
    }
    
    public abstract class ObjectFieldGenerator<TCustomField, TObj> : ObjectFieldGenerator<TObj> where TCustomField : GenerationField
    {
        protected TCustomField field;
        
        protected ObjectFieldGenerator(TCustomField field)
        {
            this.field = field;
        }
    }

    public class ObjectLeafFieldGenerator<TObj, TField> : ObjectFieldGenerator<GenerationLeafField<TObj, TField>, TObj>
    {
        private ExpressionSetterDelegate<TObj, TField> setter;
        
        public ObjectLeafFieldGenerator(GenerationLeafField<TObj, TField> field) : base(field)
        {
            setter = CustomObjectFieldMapper<TObj, TField>.GetSetter(field.info);
        }
        
        public override void GenerateField(ref TObj target, ref Random random)
        {
            if (!field.generate)
                return;
            
            TField value = field.generator.Generate(ref random);
            setter(ref target, value);
        }
    }
    
    public class ObjectNestedFieldGenerator<TObj, TField> : ObjectFieldGenerator<GenerationNestedField<TObj, TField>, TObj>
    {
        private ExpressionGetterDelegate<TObj, TField> getter;
        private ExpressionSetterDelegate<TObj, TField> setter;

        private List<ObjectFieldGenerator<TField>> children = new();
        
        public ObjectNestedFieldGenerator(GenerationNestedField<TObj, TField> field) : base(field)
        {
            getter = CustomObjectFieldMapper<TObj, TField>.GetGetter(field.info);
            setter = CustomObjectFieldMapper<TObj, TField>.GetSetter(field.info);

            CreateGenerationChildren();
        }

        private void CreateGenerationChildren()
        {
            foreach (GenerationField fieldChild in field.children)
            {
                Type fieldChildType = fieldChild.GetType().GetGenericTypeDefinition();
                Type[] fieldChildParams = { typeof(TField), fieldChild.info.FieldType };

                Type generationType;
                
                if (fieldChildType == typeof(GenerationLeafField<,>))
                    generationType = typeof(ObjectLeafFieldGenerator<,>);
                else if (fieldChildType == typeof(GenerationNestedField<,>))
                    generationType = typeof(ObjectNestedFieldGenerator<,>);
                else
                    throw new Exception();

                generationType = generationType.MakeGenericType(fieldChildParams);
                children.Add((ObjectFieldGenerator<TField>)Activator.CreateInstance(generationType, fieldChild));
            }
        }
        
        public override void GenerateField(ref TObj target, ref Random random)
        {
            if (!field.generate)
                return;
            
            TField nested = getter(target);

            foreach (ObjectFieldGenerator<TField> child in children)
                child.GenerateField(ref nested, ref random);
            
            setter(ref target, nested);
        }
    }
}