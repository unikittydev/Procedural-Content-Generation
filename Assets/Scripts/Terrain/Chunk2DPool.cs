using UnityEngine;

namespace PCG.Terrain
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ChunkWorld2D))]
    public class Chunk2DPool : ObjectPool<Chunk2D> { }
}