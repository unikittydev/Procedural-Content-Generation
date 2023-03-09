using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.TerrainGeneration
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FlatChunk : MonoBehaviour
    {
        private FlatWorld _world;
        public FlatWorld world => _world;

        private MeshFilter _meshFilter;
        public MeshFilter meshFilter => _meshFilter;

        private MeshRenderer _renderer;
        public MeshRenderer renderer => _renderer;
        
        private NativeArray<float> _density;
        public NativeArray<float> density => _density;

        private NativeArray<float3> _normals;
        public NativeArray<float3> normals => _normals;

        private NativeArray<float2> _derivs;
        public NativeArray<float2> derivs => _derivs;

        public int2 position { get; private set; }

        [SerializeField] private bool drawDerivs, drawNormals;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();
        }

        private void OnDrawGizmos()
        {
            if (drawDerivs)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < density.Length; i++)
                {
                    int2 pos = FlatChunkUtils.GetPosition(world.chunkSize, i);
                    Vector3 transformWorldPosition =
                        FlatChunkUtils.LocalToTransformWorldPosition(transform.position, pos, this[i],
                            world.chunkScale);
                    Gizmos.DrawRay(transformWorldPosition, new Vector3(-derivs[i].x, 0f, -derivs[i].y));
                }
            }

            if (drawNormals)
            {
                var normals = meshFilter.mesh.normals;
                for (int i = 0; i < density.Length; i++)
                {
                    int2 pos = FlatChunkUtils.GetPosition(world.chunkSize, i);
                    Vector3 transformWorldPosition =
                        FlatChunkUtils.LocalToTransformWorldPosition(transform.position, pos, this[i],
                            world.chunkScale);
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(transformWorldPosition, normals[i]);
                    Gizmos.color = Color.black;
                    Gizmos.DrawRay(transformWorldPosition, this.normals[i]);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(transformWorldPosition, normals[i] - (Vector3)this.normals[i]);
                }
            }
        }

        public void Init(FlatWorld world, in int2 position)
        {
            _world = world;
            this.position = position;

            int length = world.chunkSize.x * world.chunkSize.y;
            _density = new NativeArray<float>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _normals = new NativeArray<float3>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            _derivs = new NativeArray<float2>(length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            
            _meshFilter.mesh = new Mesh();
        }

        private void OnDestroy()
        {
            _density.Dispose();
            _normals.Dispose();
            _derivs.Dispose();
        }

        public float this[in int2 pos]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _density[FlatChunkUtils.GetIndex(world.chunkSize, pos)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _density[FlatChunkUtils.GetIndex(world.chunkSize, pos)] = value;
        }

        public float this[in int x, in int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _density[FlatChunkUtils.GetIndex(world.chunkSize, x, y)];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _density[FlatChunkUtils.GetIndex(world.chunkSize, x, y)] = value;
        }

        public float this[in int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _density[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _density[index] = value;
        }
    }
}
