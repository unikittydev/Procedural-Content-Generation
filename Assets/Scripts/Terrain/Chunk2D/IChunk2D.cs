using System;
using Unity.Collections;
using Unity.Mathematics;

namespace PCG.Terrain
{
    public interface IChunk2D : IDisposable
    {
        public void Create(in int2 size);
    }
    
    public interface IChunk2D<T> : IChunk2D where T : unmanaged
    {
        public NativeArray<T> content { get; }
    }
}
