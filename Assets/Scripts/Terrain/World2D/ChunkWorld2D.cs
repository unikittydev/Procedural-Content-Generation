using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Chunk2DPool))]
    [RequireComponent(typeof(ChunkManager2D))]
    [RequireComponent(typeof(Chunk2DGenerator))]
    public class ChunkWorld2D : MonoBehaviour, IEnumerable<KeyValuePair<int2, Chunk2D>>
    {
        private static readonly Dictionary<ChunkState, Color> gizmoColors = new()
        {
            { ChunkState.Unloaded, Color.grey },
            { ChunkState.Queued, Color.red },
            { ChunkState.TerrainGeneration, Color.yellow },
            { ChunkState.ComponentGeneration, Color.blue },
            { ChunkState.Loaded, Color.green },
        };
        
        private readonly Dictionary<int2, Chunk2D> chunks = new();

        [SerializeField] private Chunk2DGenerator _chunkGenerator;
        public Chunk2DGenerator chunkGenerator => _chunkGenerator;
        
        [field: SerializeField]
        public float worldHeight { get; private set; }
        
        [field: SerializeField]
        public float chunkWorldSize { get; private set; }
        [field: SerializeField]
        public int2 chunkResolution { get; private set; }

        public int chunkArraySize { get; private set; }
        
        [SerializeField] private bool _drawGizmos;
        
        public Chunk2D this[in int2 pos]
        {
            get
            {
                chunks.TryGetValue(pos, out Chunk2D chunk);
                return chunk;
            }

            set => chunks.TryAdd(pos, value);
        }

        public Chunk2D this[in int x, in int y]
        {
            get => this[new int2(x, y)];
            set => this[new int2(x, y)] = value;
        }

        public static bool dirty { get; set; }
        
        private void Start()
        {
            chunkArraySize = chunkResolution.x * chunkResolution.y;
        }

        private void Update()
        {
            if (!dirty)
                return;
            foreach (var kv in chunks)
            {
                _chunkGenerator.AddToGenerationQueue(kv.Value);
            }

            dirty = false;
        }

        public bool ContainsChunk(in int2 chunkPos) => chunks.ContainsKey(chunkPos);

        public bool RemoveChunk(in int2 chunkPos, out Chunk2D chunk) => chunks.Remove(chunkPos, out chunk);
        
        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
                return;
            
            Vector3 chunkBounds = Vector3.one * chunkWorldSize;
            
            foreach (var kv in chunks)
            {
                if (kv.Value.state == ChunkState.Unloaded)
                    continue;

                Vector3 position = new Vector3(kv.Key.x, 0f, kv.Key.y) * chunkWorldSize + chunkBounds * .5f;
                Gizmos.color = gizmoColors[kv.Value.state];
                Gizmos.DrawWireCube(position, chunkBounds);
            }
        }

        public IEnumerator<KeyValuePair<int2, Chunk2D>> GetEnumerator()
        {
            return chunks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
