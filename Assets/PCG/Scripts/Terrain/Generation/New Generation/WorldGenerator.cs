using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

namespace PCG.Terrain
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private Map2DPool mapPool;

        [SerializeReference] private GenerationStage[] stages;

        private static readonly ProfilerMarker chunkMarker = new("WorldGenerator.GenerateChunk()");
        private static readonly ProfilerMarker stageMarker = new("GenerationStage.Generate()");
        
        private readonly Queue<Chunk2D> generationQueue = new();
        
        private readonly HashSet<Chunk2D> chunkGenerationSet = new();

        private void Update()
        {
            while (generationQueue.Count > 0)
            {
                var chunk = generationQueue.Dequeue();
                var coroutine = StartCoroutine(GenerateChunk(chunk));
            }
        }
        
        public void AddToGenerationQueue(Chunk2D chunk)
        {
            generationQueue.Enqueue(chunk);
        }
        
        private IEnumerator GenerateChunk(Chunk2D chunk)
        {
            while (chunkGenerationSet.Contains(chunk))
                yield return null;
            
            chunkMarker.Begin();
            for (int i = 0; i < stages.Length; i++)
            {
                stageMarker.Begin();
                var stage = stages[i];
                stage.OnBeforeGenerate(i == 0 ? null : stages[i - 1], chunk);
                yield return StartCoroutine(stage.OnGenerate());
                stage.OnAfterGenerate();
                stageMarker.End();
            }
            chunk.state = ChunkState.Loaded;
            
            chunkGenerationSet.Remove(chunk);
            
            chunkMarker.End();
        }

        private void OnValidate()
        {
            foreach (var stage in stages)
                stage.mapPool = mapPool;
        }
    }
}