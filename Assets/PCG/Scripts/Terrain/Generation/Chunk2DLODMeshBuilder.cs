using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace PCG.Terrain.Generation
{
    public class Chunk2DLODMeshBuilder
    {
        private const MeshUpdateFlags flags = MeshUpdateFlags.DontRecalculateBounds |
                                              MeshUpdateFlags.DontValidateIndices |
                                              //MeshUpdateFlags.DontNotifyMeshUsers |
                                              MeshUpdateFlags.DontResetBoneBounds;
        
        private ushort[] triangles;
        private Vector2[] uvs;
        
        [BurstCompile]
        private struct CalculateTrianglesJob : IJob
        {
            [WriteOnly]
            public NativeArray<ushort> triangles;

            public int2 size;
            
            public void Execute()
            {
                for (int y = 0, t = 0; y < size.y - 1; y++)
                {
                    for (int x = 0; x < size.x - 1; x++, t += 6)
                    {
                        int i = y * size.x + x;
                        
                        triangles[t    ] = (ushort)(i             );
                        triangles[t + 1] = (ushort)(i + size.x    );
                        triangles[t + 2] = (ushort)(i          + 1);
                        triangles[t + 3] = (ushort)(i          + 1);
                        triangles[t + 4] = (ushort)(i + size.x    );
                        triangles[t + 5] = (ushort)(i + size.x + 1);
                    }
                }
            }
        }

        [BurstCompile]
        private struct CalculateVerticesJob : IJobFor
        {
            [ReadOnly]
            public NativeArray<float> data;

            [WriteOnly]
            public NativeArray<float3> vertices;

            public int2 size;
            
            public float3 meshScale;
            
            public void Execute(int index)
            {
                int x = index % size.x, y = index / size.x;
                
                float2 localPos = new float2(x, y);
                
                var vertex = new float3(localPos.x, data[index], localPos.y) * meshScale;
                
                vertices[index] = vertex;
            }
        }

        [BurstCompile]
        private struct CalculateUVsJob : IJobFor
        {
            [WriteOnly]
            public NativeArray<float2> uvs;

            public int2 size;
            
            public void Execute(int index)
            {
                int x = index % size.x, y = index / size.x;
                
                float2 localPos = new float2(x, y) / (size - 1);
                
                uvs[index] = localPos;
            }
        }

        public Chunk2DLODMeshBuilder(int2 chunkResolution)
        {
            int indicesCount = (chunkResolution.x - 1) * (chunkResolution.y - 1) * 6;
            NativeArray<ushort> _triangles =
                new NativeArray<ushort>(indicesCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            JobHandle handle = CalculateTriangles(chunkResolution, _triangles);

            int vertexCount = chunkResolution.x * chunkResolution.y;
            NativeArray<float2> _uvs =
                new NativeArray<float2>(vertexCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            handle = CalculateUVs(chunkResolution, _uvs, handle);
            
            handle.Complete();
                
            triangles = _triangles.ToArray();
            uvs = _uvs.Reinterpret<Vector2>().ToArray();
            _triangles.Dispose();
            _uvs.Dispose();
        }
        
        public IEnumerator Generate(Chunk2DMeshBuilder builder, Chunk2D chunk)
        {
            var arrayLength = chunk.resolution.x * chunk.resolution.y;
            
            Vector3 meshScale = Vector3.one * chunk.world.chunkWorldSize / (chunk.world.chunkResolutions[chunk.lodLevel].x - 1);
            meshScale.y = chunk.world.worldHeight;
            
            var mesh = chunk.filter.mesh;
            mesh.Clear();

            var vertices = new NativeArray<float3>(arrayLength, Allocator.Persistent,
                NativeArrayOptions.UninitializedMemory);
            JobHandle handle = CalculateVertices(chunk, meshScale, vertices, default);
            
            JobHandle.ScheduleBatchedJobs();

            yield return new WaitUntil(() => handle.IsCompleted); 
            handle.Complete();
            
            mesh.subMeshCount = 1;

            int2 chunkSize = chunk.resolution;

            // Vertices
            mesh.SetVertices(vertices, 0, vertices.Length, flags);
            if (builder.vertexNormals && !builder.recalculateNormals)
                mesh.SetNormals(chunk.GetContent<ChunkNormals2D>().content, 0, arrayLength);
            mesh.SetUVs(0, uvs, 0, arrayLength);
            
            // Indices
            mesh.SetTriangles(triangles, 0, triangles.Length, 0, false);

            Vector3 size = Vector3.Scale(meshScale, new Vector3(chunkSize.x, 1f, chunkSize.y));
            mesh.bounds = new Bounds(size * 0.5f, size);
            
            vertices.Dispose();
            
            if (builder.vertexNormals && builder.recalculateNormals)
                mesh.RecalculateNormals();
            
            if (builder.recalculateTangents)
                mesh.RecalculateTangents();
        }

        private JobHandle CalculateTriangles(in int2 size, NativeArray<ushort> triangles)
        {
            return new CalculateTrianglesJob()
            {
                triangles = triangles,
                size = size
            }.Schedule();
        }
        
        private static JobHandle CalculateVertices(Chunk2D chunk, Vector3 meshScale, NativeArray<float3> vertices, JobHandle handle)
        {
            return new CalculateVerticesJob()
            {
                data = chunk.GetContent<ChunkHeight2D>().content,
                vertices = vertices,
                size = chunk.resolution,
                meshScale = meshScale
            }.ScheduleParallel(vertices.Length, 0, handle);
        }

        private static JobHandle CalculateUVs(in int2 size, NativeArray<float2> uvs, JobHandle handle)
        {
            return new CalculateUVsJob()
            {
                uvs = uvs,
                size = size
            }.ScheduleParallel(uvs.Length, 0, handle);
        }
    }
}