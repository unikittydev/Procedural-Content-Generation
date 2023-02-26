using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public abstract class GeneratorAsset<T> : ScriptableObject, IGenerator<T>
    {
        public abstract T Generate(ref Random random);
    }

    public abstract class FloatGeneratorAsset : GeneratorAsset<float> { }

    public abstract class IntGeneratorAsset : GeneratorAsset<int> { }

    public abstract class DoubleGeneratorAsset : GeneratorAsset<double> { }
}