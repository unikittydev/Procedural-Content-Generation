using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Profiling;
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

        [SerializeField] private bool calculateLOD = true;

        private static ProfilerMarker loadMarker = new ProfilerMarker("ChunkManager2D.LoadChunks()");
        private static ProfilerMarker loadChunkMarker = new ProfilerMarker("ChunkManager2D.LoadChunk()");
        private static ProfilerMarker unloadMarker = new ProfilerMarker("ChunkManager2D.UnloadChunks()");
        
        private void Awake()
        {
            world = GetComponent<ChunkWorld2D>();
            pool = GetComponent<Chunk2DPool>();
            
            Debug.Log($"Level start at time {Time.realtimeSinceStartupAsDouble:0.000}");
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
            loadMarker.Begin();
            
            loadedPositions.Clear();
            foreach (ChunkLoader2D loader in chunkLoaders)
                LoadChunks(loader);
            
            loadMarker.End();
        }

        private void LoadChunks(ChunkLoader2D loader)
        {
            int2 loaderPositon = world.GetChunkPosition(transform.InverseTransformPoint(loader.transform.position));

            for (int x = -loader.viewRadiusInChunks; x <= loader.viewRadiusInChunks; x++)
                for (int y = -loader.viewRadiusInChunks; y <= loader.viewRadiusInChunks; y++)
                {
                    int2 chunkPos = loaderPositon + new int2(x, y);
                    LoadChunk(chunkPos, loader.transform.position, loader.lodLevelDistance);
                }
        }

        private void LoadChunk(in int2 chunkPos, Vector3 loaderPos, float[] lodLevelDistance)
        {
            loadChunkMarker.Begin();

            bool alreadyInGenerationQueue = false;
            
            if (!world.TryGetChunk(chunkPos, out Chunk2D chunk))
            {
                chunk = pool.Get(transform);
                world.chunkGenerator.AddToGenerationQueue(chunk);
                chunk.transform.position = new Vector3(chunkPos.x * world.chunkWorldSize, 0f, chunkPos.y * world.chunkWorldSize);
                world[chunkPos] = chunk;
                alreadyInGenerationQueue = true;
            }

            // If we don't use LOD, reset to maximum resolution
            // Otherwise, calculate LOD level
            int lodLevel = calculateLOD ? GetLODLevelByDistance(loaderPos, lodLevelDistance, world.GetChunkCenterBottomWorldPosition(chunkPos)) : 0;

            if (chunk.lodLevel != lodLevel)
            {
                chunk.lodLevel = lodLevel;
                if (!alreadyInGenerationQueue)
                    world.chunkGenerator.AddToGenerationQueue(chunk);
            }
            loadedPositions.Add(chunkPos);
            
            loadChunkMarker.End();
        }

        private void UnloadChunks()
        {
            unloadMarker.Begin();
            
            unloadedPositions.Clear();
            
            foreach (var kv in world)
                if (!loadedPositions.Contains(kv.Key))
                    unloadedPositions.Add(kv.Key);

            foreach (int2 chunkPos in unloadedPositions)
            {
                world.RemoveChunk(chunkPos, out Chunk2D chunk);
                pool.Add(chunk);
            }
            
            unloadMarker.End();
        }

        private int GetLODLevelByDistance(Vector3 loaderPos, float[] lodLevelDistance, Vector3 chunkPos)
        {
            for (int i = 1; i < lodLevelDistance.Length; i++)
                if ((loaderPos - chunkPos).sqrMagnitude < lodLevelDistance[i] * lodLevelDistance[i])
                    return i - 1;
            return lodLevelDistance.Length - 1;
        }
    }
}