using System.Collections;
using PCG.Generation;
using Random = Unity.Mathematics.Random;

namespace PCG.Terrain
{
    public class Chunk2DReference : GeneratorReference<Chunk2DGenerationGeneratorAsset, Chunk2D> { }

    public abstract class Chunk2DGenerationGeneratorAsset : GeneratorAsset<Chunk2D>
    {
        public Chunk2D chunk { get; set; }

        private void OnValidate()
        {
            ChunkWorld2D.dirty = true;
        }

        public abstract IEnumerator GenerateCoroutine(Random random);
    }
}