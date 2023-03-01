using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public abstract class GenerateFieldJobWrapper<TField>
        where TField : unmanaged
    {
        public abstract void Create(IntPtr ptr, int sizeOfT, int fieldOffset, IGenerator<TField> generator,
            Random random);

        public abstract JobHandle ScheduleParallel(int length, int batchLoopCount, JobHandle dependency);
    }
    
    public class GenerateFieldJobWrapper<TGen, TField>
        where TGen : unmanaged, IGenerator<TField>
        where TField : unmanaged
    {
        [BurstCompile]
        //[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
        public unsafe struct GenerateFieldJob : IJobFor
        {
            [WriteOnly]
            [NativeDisableUnsafePtrRestriction]
            [NativeDisableParallelForRestriction]
            public byte* collection;
        
            public int sizeOfT;

            public int fieldOffset;
        
            public TGen generator;
        
            public Random random;
        
            public GenerateFieldJob(
                IntPtr collection,
                int sizeOfT,
                int fieldOffset,
                TGen generator,
                Random random)
            {
                this.collection = (byte*)collection.ToPointer();
                this.sizeOfT = sizeOfT;
                this.fieldOffset = fieldOffset;
                this.generator = generator;
                this.random = random;
            }
        
            public void Execute(int index)
            {
                var random = Random.CreateFromIndex((uint)index + this.random.state);

                TField value = generator.Generate(ref random);

                int indexInCollection = index * sizeOfT + fieldOffset;
                UnsafeUtility.MemCpy(collection + indexInCollection, &value, sizeof(TField));
            }
        }

        private GenerateFieldJob job;

        public GenerateFieldJobWrapper()
        {
            
        }

        public GenerateFieldJobWrapper(IntPtr ptr, int sizeOfT, int fieldOffset, IGenerator<TField> generator,
            Random random)
        {
            job = new GenerateFieldJob(ptr, sizeOfT, fieldOffset, (TGen)generator, random);
        }
        
        public void Create(IntPtr ptr, int sizeOfT, int fieldOffset, IGenerator<TField> generator, Random random)
        {
            random = Random.CreateFromIndex(random.state + (uint)fieldOffset);
            job = new GenerateFieldJob(ptr, sizeOfT, fieldOffset, (TGen)generator, random);
        }

        public JobHandle ScheduleParallel(int length, int batchLoopCount, JobHandle dependency)
        {
            return job.ScheduleParallel(length, batchLoopCount, dependency);
        }
    }
}