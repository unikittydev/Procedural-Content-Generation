
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [DisplayName("Uniform int")]
    [System.Serializable]
    public struct UniformIntGenerator : IGenerator<int>
    {
        public int min, max;

        public int Generate(ref Random random)
        {
            return random.NextInt(min, max);
        }
    }
}
