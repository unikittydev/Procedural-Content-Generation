using Unity.Assertions;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

namespace PCG.Terrain
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(FlatWorld))]
    public class FlatChunkMeshBuilder : MonoBehaviour
    {
        private FlatWorld world;
        
        private const MeshUpdateFlags flags = MeshUpdateFlags.DontRecalculateBounds |
                                              MeshUpdateFlags.DontValidateIndices |
                                              //MeshUpdateFlags.DontNotifyMeshUsers |
                                              MeshUpdateFlags.DontResetBoneBounds;
        
        private ushort[] triangles;
        private Vector2[] uvs;
        
        private ProfilerMarker marker = new ProfilerMarker("TerrainMeshBuilder.CreateMesh");
        
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

        [SerializeField]
        private bool recalculateNormals, recalculateTangents, vertexNormals;
        private void Awake()
        {
            world = GetComponent<FlatWorld>();
        }

        private void Start()
        {
            int indicesCount = (world.chunkSize.x - 1) * (world.chunkSize.y - 1) * 6;
            NativeArray<ushort> _triangles =
                new NativeArray<ushort>(indicesCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            JobHandle handle = CalculateTriangles(world.chunkSize, _triangles);

            int vertexCount = world.chunkSize.x * world.chunkSize.y;
            NativeArray<float2> _uvs =
                new NativeArray<float2>(vertexCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            handle = CalculateUVs(world.chunkSize, _uvs, handle);
            
            handle.Complete();
                
            triangles = _triangles.ToArray();
            uvs = _uvs.Reinterpret<Vector2>().ToArray();
            _triangles.Dispose();
            _uvs.Dispose();
        }
        
        public void CreateMesh(FlatChunk flatChunk, Vector3 meshScale)
        {
            marker.Begin();
            
            var mesh = flatChunk.meshFilter.mesh;
            mesh.Clear();

            var vertices = new NativeArray<float3>(flatChunk.density.Length, Allocator.TempJob,
                NativeArrayOptions.UninitializedMemory);
            JobHandle handle = CalculateVertices(flatChunk, meshScale, vertices, default);

            handle.Complete();

            mesh.subMeshCount = 1;

            int2 chunkSize = flatChunk.world.chunkSize;

            // Vertices
            mesh.SetVertices(vertices, 0, vertices.Length, flags);
            if (vertexNormals && !recalculateNormals)
                mesh.SetNormals(flatChunk.normals, 0, flatChunk.normals.Length);
            mesh.SetUVs(0, uvs, 0, uvs.Length);
            
            // Indices
            mesh.SetTriangles(triangles, 0, triangles.Length, 0, false);

            Vector3 size = Vector3.Scale(meshScale, new Vector3(chunkSize.x, 1f, chunkSize.y));
            mesh.bounds = new Bounds(size * 0.5f, size);
            
            vertices.Dispose();
            
            if (vertexNormals && recalculateNormals)
                mesh.RecalculateNormals();
            
            if (recalculateTangents)
                mesh.RecalculateTangents();

            marker.End();
        }

        private JobHandle CalculateTriangles(in int2 size, NativeArray<ushort> triangles)
        {
            return new CalculateTrianglesJob()
            {
                triangles = triangles,
                size = size
            }.Schedule();
        }
        
        private static JobHandle CalculateVertices(FlatChunk flatChunk, Vector3 meshScale, NativeArray<float3> vertices, JobHandle handle)
        {
            return new CalculateVerticesJob()
            {
                data = flatChunk.density,
                vertices = vertices,
                size = flatChunk.world.chunkSize,
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