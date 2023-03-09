using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace PCG.TerrainGeneration
{
    [CreateAssetMenu(menuName = "PCG/Terrain/Fractal Simplex Settings", fileName = "New Fractal Simplex Settings")]
    public class FractalSimplexFlatChunkSettings : FlatChunkSettings
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance)]
        private struct GenerateNoiseJob : IJobFor
        {
            [WriteOnly]
            public NativeArray<float> density;
            [WriteOnly]
            public NativeArray<float3> normals;
            [WriteOnly]
            public NativeArray<float2> derivs;

            public int2 size;
            public float2 chunkWorldPos;

            public float2 normalScale;
            
            public Settings set;

            public void Execute(int index)
            {
                int x = index % size.x, y = index / size.x;
                
                float2 localPos = new float2(x, y) + chunkWorldPos;

                terrainHeight noise = Evaluate(localPos);
                
                density[index] = noise.value;

                float2 derivative = noise.derivative * normalScale;
                derivs[index] = derivative;
                normals[index] =  math.normalize(new float3(-derivative.x, 1f, -derivative.y));
            }
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private terrainHeight Evaluate(float2 pos)
            {
                var value = new terrainHeight();

                float amplitude = 1f, amplitudeSum = 0f, frequency = set.frequency;
                for (int i = 0; i < set.octaves; i++)
                {
                    terrainHeight octaveValue = noise.srdnoise(pos * frequency);
                    octaveValue = new terrainHeight(octaveValue.value, octaveValue.derivative * frequency);
                    octaveValue /= 1f + set.erosionPower * math.dot(octaveValue.value, octaveValue.value);
                    value += amplitude * octaveValue;

                    amplitudeSum += amplitude;
                    amplitude *= set.persistence;
                    frequency *= set.lacunarity;
                }

                value /= amplitudeSum;
                value = (value + 1f) * .5f;
                
                value = terrainHeight.pow(value, set.redistributionPower);
                
                return value;
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
        }

        public Settings settings = new ()
        {
            frequency = 1f,
            persistence = .5f,
            lacunarity = 2f,
            octaves = 1,
            redistributionPower = 1f,
            erosionPower = 0f
        };

        public override void Generate(FlatChunk chunk)
        {
            float2 normalScale = new float2(chunk.world.chunkScale.y / chunk.world.chunkScale.x,
                chunk.world.chunkScale.y / chunk.world.chunkScale.z);
            
            var job = new GenerateNoiseJob()
            {
                density = chunk.density,
                derivs = chunk.derivs,
                normals = chunk.normals,
                size = chunk.world.chunkSize,
                chunkWorldPos = chunk.position * (chunk.world.chunkSize - 1),
                set = settings,
                normalScale = normalScale
            }.ScheduleParallel(chunk.density.Length, 0, default);
            job.Complete();
        }
    }
}