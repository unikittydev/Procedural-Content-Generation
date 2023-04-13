using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace PCG.TerrainGeneration
{
    public readonly partial struct terrainDensity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator terrainDensity(in float4 a)
        {
            return new terrainDensity(a.x, a.yzw);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator +(in terrainDensity a)
        {
            return new terrainDensity
            (
                a.value,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator +(in terrainDensity a, in float b)
        {
            return new terrainDensity
            (
                a.value + b,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator +(in float a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a + b.value,
                b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator +(in terrainDensity a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a.value + b.value,
                a.derivative + b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator -(in terrainDensity a)
        {
            return new terrainDensity
            (
                -a.value,
                -a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator -(in terrainDensity a, in float b)
        {
            return new terrainDensity
            (
                a.value - b,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator -(in float a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a - b.value,
                -b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator -(in terrainDensity a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a.value - b.value,
                a.derivative - b.derivative
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator *(in terrainDensity a, in float b)
        {
            return new terrainDensity
            (
                a.value * b,
                a.derivative * b
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator *(in float a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a * b.value,
                a * b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator *(in terrainDensity a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a.value * b.value,
                a.value * b.derivative + a.derivative * b.value
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator /(in terrainDensity a, in float b)
        {
            return new terrainDensity
            (
                a.value / b,
                a.derivative / b
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator /(in float a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a / b.value,
                -a * b.derivative / (b.value * b.value)
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity operator /(in terrainDensity a, in terrainDensity b)
        {
            return new terrainDensity
            (
                a.value / b.value,
                (a.derivative * b.value - b.derivative * a.value) / (b.value * b.value)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static terrainDensity pow(in terrainDensity a, in float b)
        {
            return new terrainDensity
            (
                math.pow(a.value, b),
                b * a.derivative * math.pow(a.value, b - 1f)
            );
        }
    }
}