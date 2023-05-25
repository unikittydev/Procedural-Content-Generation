using System.Collections;
using UnityEngine;

namespace PCG.Terrain
{
    public abstract class Chunk2DGenerationStage : ScriptableObject
    {
        [SerializeField] private ChunkState state;

        public abstract void Init(Chunk2DGenerator generator);

        public virtual void OnBeforeGenerate(Chunk2D chunk)
        {
            chunk.state = state;
        }
        
        public abstract IEnumerator Generate(Chunk2D chunk);
        
        public virtual void OnAfterGenerate(Chunk2D chunk)
        {
        }
    }
}