
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [System.Serializable]
    public class UniformIntGenerator : IGenerator<int>
    {
        public int min, max;

        public int Generate(ref Random random)
        {
            return random.NextInt(min, max);
        }
    }
}
