using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    public enum ChunkState
    {
        Unloaded,
        Queued,
        TerrainGeneration,
        ComponentGeneration,
        Loaded
    }

    [Flags]
    public enum ChunkContent
    {
        Height = 1,
        Normals = 2,
    }
    
    public class Chunk2D : MonoBehaviour
    {
        public static readonly Dictionary<ChunkContent, Func<int2, IChunk2D>> contentMap = new()
        {
            { ChunkContent.Height, size => CreateContent<ChunkHeight2D>(size) },
            { ChunkContent.Normals, size => CreateContent<ChunkNormals2D>(size) },
        };

        private static readonly Dictionary<Type, ChunkContent> typeToContentMap = new()
        {
            { typeof(ChunkHeight2D), ChunkContent.Height },
            { typeof(ChunkNormals2D), ChunkContent.Normals },
        };

        [field: SerializeField, HideInInspector]
        public ChunkWorld2D world { get; set; }

        [field: SerializeField]
        public MeshFilter filter { get; set; }
        
        public ChunkState state { get; set; }

        public Dictionary<ChunkContent, IChunk2D> data { get; private set; } = new();

        private void OnEnable()
        {
            world.chunkGenerator.AddToGenerationQueue(this);
        }

        private void OnDisable()
        {
            state = ChunkState.Unloaded;
            StopAllCoroutines();
        }

        public void CreateData(ChunkContent includedContent)
        {
            foreach (ChunkContent contentType in Enum.GetValues(typeof(ChunkContent)))
                if (includedContent.HasFlag(contentType))
                    data.Add(contentType, contentMap[contentType](world.chunkResolution));
        }

        public T GetContent<T>() where T : IChunk2D, new()
        {
            var chunkContent = typeToContentMap[typeof(T)];
            return (T)data[chunkContent];
        }

        private void OnDestroy()
        {
            foreach (var kv in data)
                kv.Value.Dispose();
        }
        
        private static T CreateContent<T>(in int2 size) where T : IChunk2D, new()
        {
            T content = new T();
            content.Create(size);
            return content;
        }
    }
}