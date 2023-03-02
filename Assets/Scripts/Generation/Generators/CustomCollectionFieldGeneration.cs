using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEditor;
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public abstract class CustomCollectionFieldGeneration<TObj>
    {
        public int parentFieldOffset;
        public int localFieldOffset;

        public abstract JobHandle GenerateField<T>(INativeCollectionProvider<T> collection, ref Random random, JobHandle dependency)
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
        private abstract class JobWrapper
        {
            private Type previousGeneratorType;

            protected JobWrapper(Type generatorType)
            {
                previousGeneratorType = generatorType;
            }
            
            public bool IsValidWrapper(Type currentGeneratorType) => previousGeneratorType == currentGeneratorType;
            
            public abstract unsafe JobHandle Schedule(
                void* ptr,
                int sizeOfT,
                int fieldOffset,
                IGenerator<TField> generator,
                uint seed,
                int length,
                JobHandle dependency);
        }

        private class JobWrapper<TGen> : JobWrapper where TGen : unmanaged, IGenerator<TField>
        {
            public JobWrapper() : base(typeof(TGen)) { }
            
            public override unsafe JobHandle Schedule(
                void* ptr,
                int sizeOfT,
                int fieldOffset,
                IGenerator<TField> generator,
                uint seed,
                int length,
                JobHandle dependency)
            {
                var job = new GenerateFieldJob<TGen, TField>()
                {
                    collection = (byte*)ptr,
                    sizeOfT = sizeOfT,
                    fieldOffset = fieldOffset,
                    generator = (TGen)generator,
                    seed = seed
                };
                return job.ScheduleParallel(length, 0, dependency);
            }

        }

        private JobWrapper jobWrapper;

        public CustomCollectionLeafFieldGeneration(CustomLeafField<TObj, TField> field) : base(field)
        {
            TryCreateJobWrapper();
        }

        private bool TryCreateJobWrapper()
        {
            if (!UnsafeUtility.IsUnmanaged(field.generator.GetType()))
                return false;
            var genType = field.generator.GetType();
            var wrapperType = typeof(JobWrapper<>).MakeGenericType(typeof(TObj), typeof(TField), genType);
            jobWrapper = (JobWrapper)Activator.CreateInstance(wrapperType);
            return true;
        }

        public override unsafe JobHandle GenerateField<T>(INativeCollectionProvider<T> collection, ref Random random,
            JobHandle dependency)
        {
            if (!field.generate)
                return dependency;
            
            if (jobWrapper == null || !jobWrapper.IsValidWrapper(field.generator.GetType()))
                if (!TryCreateJobWrapper())
                    return dependency;

            void* ptr = collection.GetUnsafePtr();

            int sizeOfT = UnsafeUtility.SizeOf<T>();
            int fieldOffset = localFieldOffset + parentFieldOffset;

            return jobWrapper!.Schedule(
                ptr, 
                sizeOfT, 
                fieldOffset,
                field.generator,
                random.state,
                collection.Length,
                dependency);
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

        public override JobHandle GenerateField<T>(INativeCollectionProvider<T> collection, ref Random random, JobHandle dependency)
        {
            if (!field.generate)
                return dependency;

            NativeArray<JobHandle> handles = new(children.Count, Allocator.Temp,
                NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < children.Count; i++)
            {
                CustomCollectionFieldGeneration<TField> child = children[i];
                handles[i] = child.GenerateField(collection, ref random, dependency);
            }
            dependency = JobHandle.CombineDependencies(handles);

            return dependency;
        }
    }
}
