
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [DisplayName("Uniform float")]
    [System.Serializable]
    public struct UniformFloatGenerator : IGenerator<float>
    {
        public float min, max;
    
        public float Generate(ref Random random)
        {
            return random.NextFloat(min, max);
        }
    }
}