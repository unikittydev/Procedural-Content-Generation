using Unity.Jobs;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [System.Serializable]
    public struct UniformFloatGenerator : IGenerator<float>
    {
        public float min, max;
    
        static UniformFloatGenerator()
        {
            new GenerateFieldJobWrapper<UniformFloatGenerator, float>.GenerateFieldJob()
                .ScheduleParallel(0, 0, default).Complete();
        }
        
        public float Generate(ref Random random)
        {
            return random.NextFloat(min, max);
        }
    }
}