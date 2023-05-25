using System.Collections;
using System.Collections.Generic;
using PCG.Terrain.Generation;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace PCG.Terrain
{
    public class Chunk2DGenerator : MonoBehaviour
    {
        private readonly Queue<Chunk2D> generationQueue = new();

        [SerializeField] private List<Chunk2DGenerationStage> stages = new();

        private readonly HashSet<Chunk2D> chunkGenerationSet = new();

        private Random random;
        
        private void Awake()
        {
            random = new Random();
            foreach (var stage in stages)
                stage.Init(this);
        }

        private void Update()
        {
            while (generationQueue.Count > 0)
            {
                var chunk = generationQueue.Dequeue();
                var coroutine = StartCoroutine(GenerateChunk(chunk));
            }
        }

        public ref Random GetRandom() => ref random;

        public void AddToGenerationQueue(Chunk2D chunk)
        {
            generationQueue.Enqueue(chunk);
        }

        private IEnumerator GenerateChunk(Chunk2D chunk)
        {
            while (chunkGenerationSet.Contains(chunk))
                yield return null;
            //yield return new WaitWhile(() => chunkGenerationSet.Contains(chunk));

            chunkGenerationSet.Add(chunk);
            
            foreach (var stage in stages)
            {
                stage.OnBeforeGenerate(chunk);
                yield return StartCoroutine(stage.Generate(chunk));
                stage.OnAfterGenerate(chunk);
            }
            chunk.state = ChunkState.Loaded;

            chunkGenerationSet.Remove(chunk);
            
            Debug.Log($"{chunk.name} is loaded at time {Time.realtimeSinceStartupAsDouble:0.00}", chunk);
        }
    }
}