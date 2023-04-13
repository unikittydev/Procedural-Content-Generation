using System.Collections;
using System.Runtime.CompilerServices;
using PCG.Terrain;
using PCG.TerrainGeneration;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Sample
{
    [CreateAssetMenu(menuName = "Sample/Noise Generator", fileName = nameof(SampleChunk2DNoiseGenerator))]
    public class SampleChunk2DNoiseGenerator : Chunk2DGenerationGeneratorAsset
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance)]
        private struct GenerateNoiseJob : IJobFor
        {
            [WriteOnly]
            public NativeArray<float> density;
            [WriteOnly]
            public NativeArray<float3> normals;

            public int2 size;
            public float2 chunkWorldPos;
            public float2 positionScale;

            public float2 normalScale;
            
            public Settings set;

            public void Execute(int index)
            {
                int x = index % size.x, y = index / size.x;
                
                float2 localPos = new float2(x, y) * positionScale + chunkWorldPos;

                terrainHeight noise = Evaluate(localPos);
                
                density[index] = noise.value;

                float2 derivative = noise.derivative * normalScale;

                //float3 tangentX = new float3(0f, derivative.y, 1f);
                //float3 tangentZ = new float3(1f, derivative.x, 0f);
                normals[index] =  math.normalize(new float3(-derivative.x, 1f, -derivative.y));
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private terrainHeight Evaluate(in float2 pos)
            {
                //var value = FBM(pos);

                var warpedPos = DomainWarping(pos);
                var value = FBM(warpedPos);

                value = terrainHeight.pow(value, set.redistributionPower);
                
                return value;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private terrainHeight FBM(in float2 pos)
            {
                var value = new terrainHeight();

                float amplitude = 1f, amplitudeSum = 0f, frequency = set.frequency;
                for (int i = 0; i < set.octaves; i++)
                {
                    terrainHeight octaveValue = noise.srdnoise(pos * frequency);
                    octaveValue = new terrainHeight(octaveValue.value, octaveValue.derivative * frequency);
                    value += amplitude * octaveValue;

                    amplitudeSum += amplitude;
                    amplitude *= set.persistence;
                    frequency *= set.lacunarity;
                }

                value /= amplitudeSum;
                value = (value + 1f) * .5f;

                return value;
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private float2 DomainWarping(in float2 pos)
            {
                var q = new float2(
                    FBM(pos).value,
                    FBM(pos + set.warpingOffset).value
                );
                return pos + q * set.warpingStrenth;
            }
        }
        
        [System.Serializable]
        public struct Settings
        {
            [Range(0f, .3f)]
            public float frequency;
            [Range(0f, 1f)]
            public float persistence;
            [Range(1f, 4f)]
            public float lacunarity;
            [Range(1, 16)]
            public int octaves;
            [Range(0f, 10f)]
            public float redistributionPower;
            [Range(0f, 10f)]
            public float erosionPower;

            [Header("Domain Warping")]
            public float2 warpingOffset;
            public float warpingStrenth;
        }

        public Settings settings = new ()
        {
            frequency = 1f,
            persistence = .5f,
            lacunarity = 2f,
            octaves = 1,
            redistributionPower = 1f,
            erosionPower = 0f,
        };
        
        public override Chunk2D Generate(ref Random random)
        {
            float2 normalScale = chunk.world.worldHeight;
            
            var job = new GenerateNoiseJob()
            {
                density = chunk.GetContent<ChunkHeight2D>().content,
                normals = chunk.GetContent<ChunkNormals2D>().content,
                size = chunk.world.chunkResolution,
                chunkWorldPos = new float2(chunk.transform.position.x, chunk.transform.position.z),
                positionScale = chunk.world.chunkWorldSize / (float2)(chunk.world.chunkResolution - 1),
                set = settings,
                normalScale = normalScale
            }.ScheduleParallel(chunk.world.chunkArraySize, 0, default);
            job.Complete();

            return chunk;
        }

        public override IEnumerator GenerateCoroutine(Random random)
        {
            float2 normalScale = chunk.world.worldHeight;
            
            var job = new GenerateNoiseJob()
            {
                density = chunk.GetContent<ChunkHeight2D>().content,
                normals = chunk.GetContent<ChunkNormals2D>().content,
                size = chunk.world.chunkResolution,
                chunkWorldPos = new float2(chunk.transform.position.x, chunk.transform.position.z),
                positionScale = chunk.world.chunkWorldSize / (float2)(chunk.world.chunkResolution - 1),
                set = settings,
                normalScale = normalScale
            }.ScheduleParallel(chunk.world.chunkArraySize, 0, default);
            JobHandle.ScheduleBatchedJobs();

            yield return new WaitUntil(() => job.IsCompleted);
            job.Complete();
        }
    }
}