using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [System.Serializable]
    public abstract class GeneratorReference<TAsset, TGen> : IGenerator<TGen> where TAsset : GeneratorAsset<TGen>
    {
        [SerializeReference]
        public TAsset reference;

        public TGen Generate(ref Random random)
        {
            return reference.Generate(ref random);
        }
    }

    [DisplayName("Reference int")]
    [System.Serializable]
    public class IntGeneratorReference : GeneratorReference<IntGeneratorAsset, int> { }

    [DisplayName("Reference uint")]
    [System.Serializable]
    public class UintGeneratorReference : GeneratorReference<UintGeneratorAsset, uint> { }

    [DisplayName("Reference long")]
    [System.Serializable]
    public class LongGeneratorReference : GeneratorReference<LongGeneratorAsset, long> { }

    [DisplayName("Reference ulong")]
    [System.Serializable]
    public class UlongGeneratorReference : GeneratorReference<UlongGeneratorAsset, ulong> { }

    [DisplayName("Reference float")]
    [System.Serializable]
    public class FloatGeneratorReference : GeneratorReference<FloatGeneratorAsset, float> { }

    [DisplayName("Reference double")]
    [System.Serializable]
    public class DoubleGeneratorReference : GeneratorReference<DoubleGeneratorAsset, double> { }

    [DisplayName("Reference char")]
    [System.Serializable]
    public class CharGeneratorReference : GeneratorReference<CharGeneratorAsset, char> { }

    [DisplayName("Reference string")]
    [System.Serializable]
    public class StringGeneratorReference : GeneratorReference<StringGeneratorAsset, string> { }

    [DisplayName("Reference bool")]
    [System.Serializable]
    public class BoolGeneratorReference : GeneratorReference<BoolGeneratorAsset, bool> { }

    [DisplayName("Reference Vector2")]
    [System.Serializable]
    public class Vector2GeneratorReference : GeneratorReference<Vector2GeneratorAsset, Vector2> { }

    [DisplayName("Reference Vector2Int")]
    [System.Serializable]
    public class Vector2IntGeneratorReference : GeneratorReference<Vector2IntGeneratorAsset, Vector2Int> { }

    [DisplayName("Reference Vector3")]
    [System.Serializable]
    public class Vector3GeneratorReference : GeneratorReference<Vector3GeneratorAsset, Vector3> { }

    [DisplayName("Reference Vector3Int")]
    [System.Serializable]
    public class Vector3IntGeneratorReference : GeneratorReference<Vector3IntGeneratorAsset, Vector3Int> { }
    
}
