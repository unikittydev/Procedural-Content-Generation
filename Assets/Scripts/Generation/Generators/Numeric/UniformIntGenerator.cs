using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [System.Serializable]
    public struct UniformIntGenerator : IGenerator<int>
    {
        public int min, max;

        static UniformIntGenerator()
        {
            new GenerateFieldJobWrapper<UniformIntGenerator, int>.GenerateFieldJob()
                .ScheduleParallel(0, 0, default).Complete();
        }
        
        public int Generate(ref Random random)
        {
            return random.NextInt(min, max);
        }
    }
}
