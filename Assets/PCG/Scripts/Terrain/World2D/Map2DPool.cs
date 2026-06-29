using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    public class Map2DPool : MonoBehaviour
    {
        private static readonly Dictionary<Type, Type> mapType = new()
        {
            { typeof(ChunkHeight2D), typeof(float) },
            { typeof(ChunkNormals2D), typeof(float3) },
        };

        private static readonly Dictionary<Type, Func<IChunk2D>> mapFactory = new()
        {
            { typeof(float), () => new ChunkHeight2D() },
            { typeof(float3), () => new ChunkNormals2D() },
        };

        // Capacity of each queue of each resolution
        [SerializeField]
        private int capacity;

        [SerializeField]
        private ChunkWorld2D world;
        
        private Dictionary<Type, Queue<IChunk2D>>[] mapDictionaries;

        private void Awake()
        {
            mapDictionaries = new Dictionary<Type, Queue<IChunk2D>>[world.chunkResolutions.Length];
            for (int i = 0; i < world.chunkResolutions.Length; i++)
            {
                int2 resolution = world.chunkResolutions[i];
                mapDictionaries[i] = new Dictionary<Type, Queue<IChunk2D>>();
                AddPoolForResolution(mapDictionaries[i], resolution);
            }
        }

        private void AddPoolForResolution(Dictionary<Type, Queue<IChunk2D>> dictionary, int2 resolution)
        {
            foreach (var kv in mapFactory)
            {
                dictionary[kv.Key] = new Queue<IChunk2D>(capacity);

                for (int i = 0; i < capacity; i++)
                {
                    var map = mapFactory[kv.Key]();
                    map.Create(resolution);
                    dictionary[kv.Key].Enqueue(map);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (Dictionary<Type, Queue<IChunk2D>> mapDictionary in mapDictionaries)
                foreach (var kv in mapDictionary)
                {
                    Queue<IChunk2D> pool = kv.Value;
                    foreach (IChunk2D map in pool)
                        map.Dispose();
                }
        }

        public TChunk Get<TChunk>(int lodIndex) where TChunk : IChunk2D
        {
            Type returnType = mapType[typeof(TChunk)];
            if (mapDictionaries[lodIndex][returnType].Count == 0)
                AddNewRange<TChunk>(lodIndex, capacity);
            return (TChunk)mapDictionaries[lodIndex][returnType].Dequeue();
        }

        public IChunk2D Get(Type type, int lodIndex)
        {
            Type returnType = mapType[type];
            if (mapDictionaries[lodIndex][returnType].Count == 0)
                AddNewRange(type, lodIndex, capacity);
            return mapDictionaries[lodIndex][returnType].Dequeue();
        }

        public void Add<TChunk>(int lodIndex, TChunk map) where TChunk : IChunk2D
        {
            Type returnType = mapType[typeof(TChunk)];
            mapDictionaries[lodIndex][returnType].Enqueue(map);
        }

        public void Add(Type type, int lodIndex, IChunk2D map)
        {
            Type returnType = mapType[type];
            mapDictionaries[lodIndex][returnType].Enqueue(map);
        }
        
        private void AddNew<TChunk>(int lodIndex) where TChunk : IChunk2D
        {
            Type returnType = mapType[typeof(TChunk)];
            var map = mapFactory[returnType]();
            map.Create(world.chunkResolutions[lodIndex]);
            mapDictionaries[lodIndex][returnType].Enqueue(map);
        }
        
        private void AddNew(Type type, int lodIndex)
        {
            Type returnType = mapType[type];
            var map = mapFactory[returnType]();
            map.Create(world.chunkResolutions[lodIndex]);
            mapDictionaries[lodIndex][returnType].Enqueue(map);
        }

        private void AddNewRange<TChunk>(int lodIndex, int count) where TChunk : IChunk2D
        {
            for (int i = 0; i < count; i++)
                AddNew<TChunk>(lodIndex);
        }
        
        private void AddNewRange(Type type, int lodIndex, int count)
        {
            for (int i = 0; i < count; i++)
                AddNew(type, lodIndex);
        }
    }
}