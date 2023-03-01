
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [System.Serializable]
    public class UniformDoubleGenerator : IGenerator<double>
    {
        public double min, max;

        public double Generate(ref Random random)
        {
            return random.NextDouble(min, max);
        }
    }
}
