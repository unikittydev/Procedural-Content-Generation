using System.Collections;
using UnityEngine;

namespace PCG.Terrain
{
    [CreateAssetMenu(menuName = "PCG/Terrain/Generator", fileName = nameof(Chunk2DGenerationGeneratorStage))]
    public class Chunk2DGenerationGeneratorStage : Chunk2DGenerationStage
    {
        [SerializeField]
        private Chunk2DGenerationGeneratorAsset asset;

        private Chunk2DGenerator generator;
        
        public override void Init(Chunk2DGenerator generator)
        {
            this.generator = generator;
        }

        public override IEnumerator Generate()
        {
            asset.chunk = currentChunk;

            yield return asset.GenerateCoroutine(generator.GetRandom());
        }
    }
}