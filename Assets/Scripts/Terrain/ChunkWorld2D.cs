using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Chunk2DPool))]
    [RequireComponent(typeof(ChunkManager2D))]
    public class ChunkWorld2D : MonoBehaviour, IEnumerable<KeyValuePair<int2, Chunk2D>>
    {
        private readonly Dictionary<int2, Chunk2D> chunks = new();

        [field: SerializeField]
        public float chunkSize { get; private set; }

        [SerializeField] private bool drawGizmos;

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

        public bool ContainsChunk(in int2 chunkPos) => chunks.ContainsKey(chunkPos);

        public bool RemoveChunk(in int2 chunkPos, out Chunk2D chunk) => chunks.Remove(chunkPos, out chunk);
        
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
                return;
            
            Gizmos.color = Color.black;
            Vector3 chunkBounds = Vector3.one * chunkSize;
            
            foreach (var kv in chunks)
            {
                Vector3 position = new Vector3(kv.Key.x, 0f, kv.Key.y) * chunkSize + chunkBounds * .5f;
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
