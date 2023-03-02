using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [System.Serializable]
    public struct UniformDoubleGenerator : IGenerator<double>
    {
        public double min, max;

        static UniformDoubleGenerator()
        {
            new GenerateFieldJob<UniformDoubleGenerator, double>().ScheduleParallel(0, 0, default).Complete();
        }
        
        public double Generate(ref Random random)
        {
            return random.NextDouble(min, max);
        }
    }
}
