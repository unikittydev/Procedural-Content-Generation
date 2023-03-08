using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace PCG.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly partial struct terrainHeight
    {
        public readonly float value;
        public readonly float2 derivative;

        public terrainHeight(in float value, in float2 derivative)
        {
            this.value = value;
            this.derivative = derivative;
        }
    }   
}
