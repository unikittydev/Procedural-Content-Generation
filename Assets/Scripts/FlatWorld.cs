using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.TerrainGeneration
{
    [DisallowMultipleComponent]
    public class FlatWorld : MonoBehaviour
    {
        private Dictionary<int2, FlatChunk> chunks = new ();

        [field: SerializeField] 
        public int2 chunkSize { get; set; }
        [field: SerializeField]
        public Vector3 chunkScale { get; set; }

        [SerializeField] private FlatChunk chunkPrefab;

        public FlatChunk this[in int2 pos]
        {
            get
            {
                if (chunks.ContainsKey(pos))
                    return chunks[pos];
                return null;
            }
            set
            {
                if (!chunks.ContainsKey(pos))
                    chunks.Add(pos, value);
                else
                    chunks[pos] = value;
            } 
        }

        public FlatChunk this[in int x, in int y]
        {
            get => this[new int2(x, y)];
            set => this[new int2(x, y)] = value;
        }

        public FlatChunk CreateChunk(in int2 pos)
        {
            int2 worldPos = (chunkSize - 1) * pos;
            Vector3 transformWorldPos = Vector3.Scale(new Vector3(worldPos.x, 0, worldPos.y), chunkScale);

            var chunk = Instantiate(chunkPrefab, transformWorldPos, Quaternion.identity, transform);
            chunk.Init(this, pos);
            this[pos] = chunk;

            return chunk;
        }

        public void RemoveChunk(in int2 pos)
        {
            if (chunks.Remove(pos, out FlatChunk chunk)) 
                Destroy(chunk);
        }
    }
}