using System.Collections;
using PCG.Terrain.Generation;
using UnityEngine;

namespace PCG.Terrain
{
    [CreateAssetMenu(menuName = "PCG/Terrain/Mesh Builder 2D", fileName = nameof(Chunk2DMeshBuilder))]
    public class Chunk2DMeshBuilder : Chunk2DGenerationStage
    {
        private Chunk2DLODMeshBuilder[] lodBuilders;
        
        [field: SerializeField]
        public bool recalculateNormals { get; set; }
        [field: SerializeField]
        public bool recalculateTangents { get; set; }
        [field: SerializeField]
        public bool vertexNormals { get; set; }
        
        public override void Init(Chunk2DGenerator generator)
        {
            var world = generator.GetComponent<ChunkWorld2D>();
            int levelCount = world.chunkResolutions.Length;
            lodBuilders = new Chunk2DLODMeshBuilder[levelCount];

            for (int i = 0; i < lodBuilders.Length; i++)
                lodBuilders[i] = new Chunk2DLODMeshBuilder(world.chunkResolutions[i]);
        }

        public override IEnumerator Generate(Chunk2D chunk)
        {
            yield return lodBuilders[chunk.lodLevel].Generate(this, chunk);
        }
    }
}