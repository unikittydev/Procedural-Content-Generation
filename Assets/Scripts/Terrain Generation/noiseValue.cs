using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace PCG.Terrain
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct noiseValue
    {
        public readonly float value;
        public readonly float2 derivative;

        public noiseValue(in float value, in float2 derivative)
        {
            this.value = value;
            this.derivative = derivative;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator noiseValue(in float3 a)
        {
            return new noiseValue(a.x, a.yz);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator +(in noiseValue a)
        {
            return new noiseValue
            (
                a.value,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator +(in noiseValue a, in float b)
        {
            return new noiseValue
            (
                a.value + b,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator +(in float a, in noiseValue b)
        {
            return new noiseValue
            (
                a + b.value,
                b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator +(in noiseValue a, in noiseValue b)
        {
            return new noiseValue
            (
                a.value + b.value,
                a.derivative + b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator -(in noiseValue a)
        {
            return new noiseValue
            (
                -a.value,
                -a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator -(in noiseValue a, in float b)
        {
            return new noiseValue
            (
                a.value - b,
                a.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator -(in float a, in noiseValue b)
        {
            return new noiseValue
            (
                a - b.value,
                -b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator -(in noiseValue a, in noiseValue b)
        {
            return new noiseValue
            (
                a.value - b.value,
                a.derivative - b.derivative
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator *(in noiseValue a, in float b)
        {
            return new noiseValue
            (
                a.value * b,
                a.derivative * b
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator *(in float a, in noiseValue b)
        {
            return new noiseValue
            (
                a * b.value,
                a * b.derivative
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator *(in noiseValue a, in noiseValue b)
        {
            return new noiseValue
            (
                a.value * b.value,
                a.value * b.derivative + a.derivative * b.value
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator /(in noiseValue a, in float b)
        {
            return new noiseValue
            (
                a.value / b,
                a.derivative / b
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator /(in float a, in noiseValue b)
        {
            return new noiseValue
            (
                a / b.value,
                -a * b.derivative / (b.value * b.value)
            );
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue operator /(in noiseValue a, in noiseValue b)
        {
            return new noiseValue
            (
                a.value / b.value,
                (a.derivative * b.value - b.derivative * a.value) / (b.value * b.value)
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static noiseValue pow(in noiseValue a, in float b)
        {
            return new noiseValue
            (
                math.pow(a.value, b),
                b * a.derivative * math.pow(a.value, b - 1f)
            );
        }
    }   
}
