using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, DisableSafetyChecks = true,
        OptimizeFor = OptimizeFor.Performance)]
    public unsafe struct GenerateFieldJob<TGen, TField> : IJobFor
        where TGen : unmanaged, IGenerator<TField>
        where TField : unmanaged
    {
        [WriteOnly] [NativeDisableUnsafePtrRestriction] [NativeDisableParallelForRestriction]
        public byte* collection;

        public int sizeOfT;

        public int fieldOffset;

        public TGen generator;

        public uint seed;

        public void Execute(int index)
        {
            uint indexInCollection = (uint)(index * sizeOfT + fieldOffset);
            
            var random = Random.CreateFromIndex(indexInCollection + seed);
            TField value = generator.Generate(ref random);

            UnsafeUtility.MemCpy(collection + indexInCollection, &value, sizeof(TField));
        }
    }
}