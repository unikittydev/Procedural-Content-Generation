using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace PCG.TerrainGeneration
{
    public readonly partial struct terrainHeight
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator terrainHeight(in float3 a)
        {
            return new terrainHeight(a.x, a.yz);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator +(in terrainHeight a)
        {
            return new terrainHeight
            (
                a.value,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator +(in terrainHeight a, in float b)
        {
            return new terrainHeight
            (
                a.value + b,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator +(in float a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a + b.value,
                b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator +(in terrainHeight a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a.value + b.value,
                a.derivative + b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator -(in terrainHeight a)
        {
            return new terrainHeight
            (
                -a.value,
                -a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator -(in terrainHeight a, in float b)
        {
            return new terrainHeight
            (
                a.value - b,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator -(in float a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a - b.value,
                -b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator -(in terrainHeight a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a.value - b.value,
                a.derivative - b.derivative
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator *(in terrainHeight a, in float b)
        {
            return new terrainHeight
            (
                a.value * b,
                a.derivative * b
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator *(in float a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a * b.value,
                a * b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator *(in terrainHeight a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a.value * b.value,
                a.value * b.derivative + a.derivative * b.value
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator /(in terrainHeight a, in float b)
        {
            return new terrainHeight
            (
                a.value / b,
                a.derivative / b
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator /(in float a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a / b.value,
                -a * b.derivative / (b.value * b.value)
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight operator /(in terrainHeight a, in terrainHeight b)
        {
            return new terrainHeight
            (
                a.value / b.value,
                (a.derivative * b.value - b.derivative * a.value) / (b.value * b.value)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainHeight pow(in terrainHeight a, in float b)
        {
            return new terrainHeight
            (
                math.pow(a.value, b),
                b * a.derivative * math.pow(a.value, b - 1f)
            );
        }
    }
}