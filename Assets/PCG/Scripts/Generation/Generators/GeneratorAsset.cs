using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public abstract class GeneratorAsset<T> : ScriptableObject, IGenerator<T>
    {
        public abstract T Generate(ref Random random);
    }

    public abstract class FloatGeneratorAsset : GeneratorAsset<float> { }
    
    public abstract class DoubleGeneratorAsset : GeneratorAsset<double> { }

    public abstract class IntGeneratorAsset : GeneratorAsset<int> { }

    public abstract class UintGeneratorAsset : GeneratorAsset<uint> { }
    
    public abstract class LongGeneratorAsset : GeneratorAsset<long> { }
    
    public abstract class UlongGeneratorAsset : GeneratorAsset<ulong> { }
    
    public abstract class CharGeneratorAsset : GeneratorAsset<char> { }
    
    public abstract class StringGeneratorAsset : GeneratorAsset<string> { }
    
    public abstract class BoolGeneratorAsset : GeneratorAsset<bool> { }
    
    public abstract class Vector3GeneratorAsset : GeneratorAsset<Vector3> { }
    
    public abstract class Vector2GeneratorAsset : GeneratorAsset<Vector2> { }
    
    public abstract class Vector3IntGeneratorAsset : GeneratorAsset<Vector3Int> { }
    
    public abstract class Vector2IntGeneratorAsset : GeneratorAsset<Vector2Int> { }
}