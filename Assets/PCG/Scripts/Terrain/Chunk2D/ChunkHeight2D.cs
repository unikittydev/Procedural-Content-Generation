using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace PCG.Terrain
{
    public struct ChunkHeight2D : IChunk2D<float>
    {
        public NativeArray<float> content
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        public void Create(in int2 size)
        {
            content = new NativeArray<float>(size.x * size.y, Allocator.Persistent);
        }
        
        public void Dispose()
        {
            content.Dispose();
        }
    }
}