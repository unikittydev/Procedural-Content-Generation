using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace PCG.Terrain
{
    public struct ChunkNormals2D : IChunk2D<float3>
    {
        public NativeArray<float3> content
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            private set;
        }

        public void Create(in int2 size)
        {
            content = new NativeArray<float3>(size.x * size.y, Allocator.Persistent);
        }
        
        public void Dispose()
        {
            content.Dispose();
        }
    }
}