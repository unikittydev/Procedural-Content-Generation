using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [CreateAssetMenu(menuName = "PCG/" + nameof(FancyDoubleGenerator), fileName = nameof(FancyDoubleGenerator))]
    public class FancyDoubleGenerator : DoubleGeneratorAsset
    {
        public override double Generate(ref Random random)
        {
            return random.NextDouble();
        }
    }
}
