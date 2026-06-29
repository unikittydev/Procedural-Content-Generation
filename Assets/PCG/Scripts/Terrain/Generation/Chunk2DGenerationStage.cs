using System.Collections;
using UnityEngine;

namespace PCG.Terrain
{
    public abstract class Chunk2DGenerationStage : ScriptableObject
    {
        [field: SerializeField]
        public ChunkState state { get; private set; }

        public Chunk2D currentChunk { get; set; }
        
        public abstract void Init(Chunk2DGenerator generator);

        public abstract IEnumerator Generate();
    }
}