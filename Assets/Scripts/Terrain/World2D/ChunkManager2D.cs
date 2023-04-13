using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ChunkWorld2D))]
    public class ChunkManager2D : MonoBehaviour
    {
        private ChunkWorld2D world;
        private Chunk2DPool pool;
        
        private readonly HashSet<int2> loadedPositions = new();
        private readonly HashSet<int2> unloadedPositions = new();
        
        [SerializeField]
        private List<ChunkLoader2D> chunkLoaders = new();

        private void Awake()
        {
            world = GetComponent<ChunkWorld2D>();
            pool = GetComponent<Chunk2DPool>();
        }

        public void AddChunkLoader(ChunkLoader2D loader)
        {
            chunkLoaders.Add(loader);
        }

        public bool RemoveChunkLoader(ChunkLoader2D loader)
        {
            return chunkLoaders.Remove(loader);
        }
        private void Update()
        {
            LoadChunks();
            UnloadChunks();
        }

        private void LoadChunks()
        {
            loadedPositions.Clear();
            foreach (ChunkLoader2D loader in chunkLoaders)
                LoadChunks(loader);
        }

        private void LoadChunks(ChunkLoader2D loader)
        {
            int2 loaderPositon = world.GetChunkPosition(transform.InverseTransformPoint(loader.transform.position));

            for (int x = -loader.viewRadiusInChunks; x <= loader.viewRadiusInChunks; x++)
                for (int y = -loader.viewRadiusInChunks; y <= loader.viewRadiusInChunks; y++)
                {
                    int2 chunkPos = loaderPositon + new int2(x, y);
                    if (!world.ContainsChunk(chunkPos))
                    {
                        Chunk2D chunk = pool.Get(transform);
                        chunk.transform.position = new Vector3(chunkPos.x * world.chunkWorldSize, 0f, chunkPos.y * world.chunkWorldSize);
                        world[chunkPos] = chunk;
                    }

                    loadedPositions.Add(chunkPos);
                }
        }

        private void UnloadChunks()
        {
            unloadedPositions.Clear();
            
            foreach (var kv in world)
                if (!loadedPositions.Contains(kv.Key))
                    unloadedPositions.Add(kv.Key);

            foreach (int2 chunkPos in unloadedPositions)
            {
                world.RemoveChunk(chunkPos, out Chunk2D chunk);
                pool.Add(chunk);
            }
        }
    }
}