using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace PCG.Generation
{
    public delegate TField ExpressionGetterDelegate<in TObj, out TField>(TObj target);

    public delegate void ExpressionSetterDelegate<TObj, in TField>(ref TObj target, TField value);

    public static class CustomObjectFieldMapper<TObj, TField>
    {
        private static Dictionary<FieldInfo, ExpressionGetterDelegate<TObj, TField>> fieldGetters = new();

        private static Dictionary<FieldInfo, ExpressionSetterDelegate<TObj, TField>> fieldSetters = new();

        public static ExpressionGetterDelegate<TObj, TField> GetGetter(FieldInfo field)
        {
            if (!fieldGetters.ContainsKey(field))
                fieldGetters.Add(field, CreateGetter(field));

            return fieldGetters[field];
        }

        public static ExpressionSetterDelegate<TObj, TField> GetSetter(FieldInfo field)
        {
            if (!fieldSetters.ContainsKey(field))
                fieldSetters.Add(field, CreateSetter(field));

            return fieldSetters[field];
        }

        private static ExpressionGetterDelegate<TObj, TField> CreateGetter(FieldInfo field)
        {
            var targetExp = Expression.Parameter(typeof(TObj), "target");

            var fieldExp = Expression.Field(targetExp, field);

            return Expression.Lambda<ExpressionGetterDelegate<TObj, TField>>(fieldExp, targetExp).Compile();
        }

        private static ExpressionSetterDelegate<TObj, TField> CreateSetter(FieldInfo field)
        {
            var targetExp = Expression.Parameter(typeof(TObj).MakeByRefType(), "target");
            var valueExp = Expression.Parameter(field.FieldType, "value");

            var fieldExp = Expression.Field(targetExp, field);
            var assignExp = Expression.Assign(fieldExp, valueExp);

            return Expression.Lambda<ExpressionSetterDelegate<TObj, TField>>
                (assignExp, targetExp, valueExp).Compile();
        }
    }
}
