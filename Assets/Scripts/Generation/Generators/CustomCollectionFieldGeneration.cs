using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Codice.Client.BaseCommands.EventTracking;
using Codice.CM.Common;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public abstract class CustomCollectionFieldGeneration<TObj>
    {
        public int parentFieldOffset;
        public int localFieldOffset;

        public abstract JobHandle GenerateField<T>(NativeArray<T> collection, ref Random random, JobHandle dependency)
            where T : unmanaged;
    }

    public abstract class CustomCollectionFieldGeneration<TCustomField, TObj>
        : CustomCollectionFieldGeneration<TObj>
        where TCustomField : CustomField
    {
        protected TCustomField field;

        protected CustomCollectionFieldGeneration(TCustomField field)
        {
            this.field = field;
            localFieldOffset = UnsafeUtility.GetFieldOffset(field.info);
        }

        protected CustomCollectionFieldGeneration(TCustomField field, int overrideLocalFieldOffset)
        {
            this.field = field;
            localFieldOffset = overrideLocalFieldOffset;
        }
    }

    public class CustomCollectionLeafFieldGeneration<TObj, TField>
        : CustomCollectionFieldGeneration<CustomLeafField<TObj, TField>, TObj>
        where TField : unmanaged
    {
        public CustomCollectionLeafFieldGeneration(CustomLeafField<TObj, TField> field) : base(field)
        {
        }

        public override JobHandle GenerateField<T>(NativeArray<T> collection, ref Random random, JobHandle dependency)
        {
            if (!field.generate)
                return dependency;
            
            unsafe
            {
                IntPtr ptr = new IntPtr(collection.GetUnsafePtr());
                
                int sizeOfT = UnsafeUtility.SizeOf<T>();
                int fieldOffset = localFieldOffset + parentFieldOffset;
                var generator = field.generator;

                var genType = field.generator.GetType();
                var wrapperType = typeof(GenerateFieldJobWrapper<,>).MakeGenericType(genType, typeof(TField));

                    IEnumerable<ParameterExpression> args = new ParameterExpression[]
                {
                    Expression.Parameter(typeof(IntPtr), "ptr"),
                    Expression.Parameter(typeof(int), "sizeOfT"),
                    Expression.Parameter(typeof(int), "fieldOffset"),
                    Expression.Parameter(typeof(IGenerator<TField>), "generator"),
                    Expression.Parameter(typeof(Random), "random")
                };
                var exprCtor = Expression.New(wrapperType.GetConstructor(new[] { typeof(IntPtr), typeof(int), typeof(int), typeof(IGenerator<TField>), typeof(Random) })!, args);

                var lambda = Expression.Lambda(exprCtor, args).Compile();
                var test = lambda.DynamicInvoke(
                    ptr,
                    sizeOfT,
                    fieldOffset,
                    generator,
                    random);

                IEnumerable<ParameterExpression> methodArgs = new[]
                {
                    Expression.Parameter(typeof(int), "length"),
                    Expression.Parameter(typeof(int), "batchLoopCount"),
                    Expression.Parameter(typeof(JobHandle), "dependency")
                };
                var exprTarget = Expression.Parameter(test.GetType(), "test");
                var exprCall =
                    Expression.Call(exprTarget,
                        wrapperType.GetMethod("ScheduleParallel", BindingFlags.Instance | BindingFlags.Public)!,
                        methodArgs);
                var methodLambda = Expression.Lambda(exprCall, new [] { exprTarget }.Concat(methodArgs)).Compile();
                
                JobHandle handle = (JobHandle)methodLambda.DynamicInvoke(test, collection.Length, 0, dependency);


                return handle;
            }
        }
    }

    public class CustomCollectionNestedFieldGeneration<TObj, TField>
        : CustomCollectionFieldGeneration<CustomNestedField<TObj, TField>, TObj>
    {
        private List<CustomCollectionFieldGeneration<TField>> children = new();
        
        public CustomCollectionNestedFieldGeneration(CustomNestedField<TObj, TField> field) : base(field)
        {
            CreateGenerationChildren();
        }
        
        public CustomCollectionNestedFieldGeneration(CustomNestedField<TObj, TField> field, int overrideLocalFieldOffset) : base(field, overrideLocalFieldOffset)
        {
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
                    generationType = typeof(CustomCollectionLeafFieldGeneration<,>);
                else if (fieldChildType == typeof(CustomNestedField<,>))
                    generationType = typeof(CustomCollectionNestedFieldGeneration<,>);
                else
                    throw new Exception();

                generationType = generationType.MakeGenericType(fieldChildParams);
                var child = (CustomCollectionFieldGeneration<TField>)Activator.CreateInstance(generationType,
                    fieldChild);
                child.parentFieldOffset += localFieldOffset;

                children.Add(child);
            }
        }

        public override JobHandle GenerateField<T>(NativeArray<T> collection, ref Random random, JobHandle dependency)
        {
            if (!field.generate)
                return dependency;

            // Объединить зависимости
            foreach (CustomCollectionFieldGeneration<TField> child in children)
                dependency = child.GenerateField(collection, ref random, dependency);

            return dependency;
        }
    }
}
