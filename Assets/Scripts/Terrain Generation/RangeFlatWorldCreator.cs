using System.Collections;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    [RequireComponent(typeof(FlatWorld), typeof(FlatChunkMeshBuilder))]
    public class RangeFlatWorldCreator : MonoBehaviour
    {
        [SerializeField] private int2 startSize = new int2(1, 1);

        private FlatWorld world;
        private FlatChunkMeshBuilder meshBuilder;
        
        [SerializeField]
        private FlatChunkSettings generator;

        private TextureManager textureManager;
        
        [SerializeField] private float targetFPS = 60f;
        private float targetDeltaTime;

        private Coroutine generationCoroutine;

        private Stopwatch sw = new();
        
        private void Awake()
        {
            world = GetComponent<FlatWorld>();
            meshBuilder = GetComponent<FlatChunkMeshBuilder>();
            textureManager = GetComponent<TextureManager>();
            targetDeltaTime = 1000f / targetFPS;
        }

        private void Start()
        {
            CreateChunks();
            generationCoroutine = StartCoroutine(Generate());
        }

        private void Update()
        {
            if (generator.dirty)
            {
                generator.dirty = false;
                
                if (generationCoroutine != null)
                    StopCoroutine(generationCoroutine);
                generationCoroutine = StartCoroutine(Generate());
            }
        }

        private void CreateChunks()
        {
            int2 half = startSize / 2;
            for (int x = -half.x; x < startSize.x - half.x; x++)
                for (int y = -half.y; y < startSize.y - half.y; y++)
                {
                    int2 pos = new int2(x, y);
                    
                    world.CreateChunk(pos);
                }
        }
        
        private IEnumerator Generate()
        {
            float delta = 0f;

            int2 half = startSize / 2;
            for (int x = -half.x; x < startSize.x - half.x; x++)
                for (int y = -half.y; y < startSize.y - half.y; y++)
                {
                    if (delta >= targetDeltaTime)
                    {
                        yield return new WaitForSeconds(delta * .001f);
                        delta = 0f;
                    }
                    
                    sw.Restart();
                    var chunk = world[x, y];
                    generator.Generate(chunk);
                    meshBuilder.CreateMesh(chunk, world.chunkScale);
                    sw.Stop();
                    
                    delta += (float)sw.Elapsed.TotalMilliseconds;
                }
        }
    }
}