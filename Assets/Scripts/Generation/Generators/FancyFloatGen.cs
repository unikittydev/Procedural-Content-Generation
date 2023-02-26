using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [CreateAssetMenu(menuName = "PCG/Test Float", fileName = nameof(FancyFloatGen))]
    public class FancyFloatGen : FloatGeneratorAsset
    {
        public override float Generate(ref Random random)
        {
            return random.NextFloat();
        }
    }
}
