using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace PCG.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly partial struct terrainDensity
    {
        public readonly float value;
        public readonly float3 derivative;

        public terrainDensity(in float value, in float3 derivative)
        {
            this.value = value;
            this.derivative = derivative;
        }
    }
}