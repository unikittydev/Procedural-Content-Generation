using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [CreateAssetMenu(menuName = "PCG/Test Int", fileName = nameof(FancyIntGen))]
    public class FancyIntGen : IntGeneratorAsset
    {
        public override int Generate(ref Random random)
        {
            return random.NextInt();
        }
    }
}

