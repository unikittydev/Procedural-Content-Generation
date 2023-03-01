using System;
using System.Collections.Generic;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public abstract class CustomObjectFieldGeneration<TObj>
    {
        public abstract void GenerateField(ref TObj target, ref Random random);
    }
    
    public abstract class CustomObjectFieldGeneration<TCustomField, TObj> : CustomObjectFieldGeneration<TObj> where TCustomField : CustomField
    {
        protected TCustomField field;
        
        protected CustomObjectFieldGeneration(TCustomField field)
        {
            this.field = field;
        }
    }

    public class CustomObjectLeafFieldGeneration<TObj, TField> : CustomObjectFieldGeneration<CustomLeafField<TObj, TField>, TObj>
    {
        private ExpressionSetterDelegate<TObj, TField> setter;
        
        public CustomObjectLeafFieldGeneration(CustomLeafField<TObj, TField> field) : base(field)
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
    
    public class CustomObjectNestedFieldGeneration<TObj, TField> : CustomObjectFieldGeneration<CustomNestedField<TObj, TField>, TObj>
    {
        private ExpressionGetterDelegate<TObj, TField> getter;
        private ExpressionSetterDelegate<TObj, TField> setter;

        private List<CustomObjectFieldGeneration<TField>> children = new();
        
        public CustomObjectNestedFieldGeneration(CustomNestedField<TObj, TField> field) : base(field)
        {
            getter = CustomObjectFieldMapper<TObj, TField>.GetGetter(field.info);
            setter = CustomObjectFieldMapper<TObj, TField>.GetSetter(field.info);

            CreateGenerationChildren();
        }

        private void CreateGenerationChildren()
        {
            foreach (CustomField fieldChild in field.children)
            {
                Type fieldChildType = fieldChild.GetType().GetGenericTypeDefinition();
                Type[] fieldChildParams = { typeof(TField), fieldChild.info.FieldType };

                Type generationType;
                
                if (fieldChildType == typeof(CustomLeafField<,>))
                    generationType = typeof(CustomObjectLeafFieldGeneration<,>);
                else if (fieldChildType == typeof(CustomNestedField<,>))
                    generationType = typeof(CustomObjectNestedFieldGeneration<,>);
                else
                    throw new Exception();

                generationType = generationType.MakeGenericType(fieldChildParams);
                children.Add((CustomObjectFieldGeneration<TField>)Activator.CreateInstance(generationType, fieldChild));
            }
        }
        
        public override void GenerateField(ref TObj target, ref Random random)
        {
            if (!field.generate)
                return;
            
            TField nested = getter(target);

            foreach (CustomObjectFieldGeneration<TField> child in children)
                child.GenerateField(ref nested, ref random);
            
            setter(ref target, nested);
        }
    }
}