using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [System.Serializable]
    public class GeneratorReference<TAsset, TGen> : IGenerator<TGen> where TAsset : GeneratorAsset<TGen>
    {
        [SerializeReference]
        public TAsset reference;

        public TGen Generate(ref Random random)
        {
            return reference.Generate(ref random);
        }
    }

    [System.Serializable]
    public class FloatGeneratorReference : GeneratorReference<FloatGeneratorAsset, float> { }

    [System.Serializable]
    public class IntGeneratorReference : GeneratorReference<IntGeneratorAsset, int> { }

    [System.Serializable]
    public class DoubleGeneratorReference : GeneratorReference<DoubleGeneratorAsset, double> { }
}
