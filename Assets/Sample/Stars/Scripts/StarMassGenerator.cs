using PCG;
using PCG.Generation;
using Unity.Jobs;
using Unity.Mathematics;

using Random = Unity.Mathematics.Random;

[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<StarMassGenerator, float>))]

namespace PCG.Generation
{
    [DisplayName("Mass distribution")]
    [System.Serializable]
    public struct StarMassGenerator : IGenerator<float>
    {
        public float min;
        public float max;
        public float a;
        public float b;
        
        public float Generate(ref Random random)
        {
            var mass = random.NextFloat(min, max);
            return a / math.pow(mass, b);
        }
    }
}
