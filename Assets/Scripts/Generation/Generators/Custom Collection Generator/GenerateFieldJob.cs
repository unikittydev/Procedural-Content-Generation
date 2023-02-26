using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    //[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
    public unsafe struct GenerateFieldJob<TField> : IJobFor where TField : unmanaged
    {
        public byte* collection;

        public int fieldOffset;

        public IGenerator<TField> generator;

        public int sizeOfT;

        public Random random;
        
        public void Execute(int index)
        {
            //random = Random.CreateFromIndex((uint)index);

            TField value = generator.Generate(ref random);

            int indexInCollection = index * sizeOfT + fieldOffset;
            UnsafeUtility.MemCpy(collection + indexInCollection, &value, sizeof(TField));
        }
    }
}