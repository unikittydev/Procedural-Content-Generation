using Unity.Mathematics;

namespace PCG.Generation
{
    [DisplayName("Uniform float3")]
    [System.Serializable]
    public struct UniformFloat3Generator : IGenerator<float3>
    {
        public float3 min, max;
    
        public float3 Generate(ref Random random)
        {
            return random.NextFloat3(min, max);
        }
    }
}