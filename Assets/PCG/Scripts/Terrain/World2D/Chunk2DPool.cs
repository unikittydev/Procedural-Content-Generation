using UnityEngine;

namespace PCG.Terrain
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ChunkWorld2D))]
    public class Chunk2DPool : ObjectPool<Chunk2D>
    {
        [SerializeField] private ChunkContent includedContent;

        [SerializeField] private ChunkWorld2D world;
        
        protected override void InitPrefab()
        {
            prefab.world = world;
        }

        protected override Chunk2D AddInstance()
        {
            Chunk2D chunk = base.AddInstance();
            chunk.CreateData(includedContent);
            return chunk;
        }
    }
}