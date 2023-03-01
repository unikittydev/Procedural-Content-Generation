using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public class CustomCollectionGenerator<T> : GeneratorAsset<IEnumerable<T>>, IGenerateButtonCallback where T : unmanaged
    {
        [SerializeField]
        private SeedSettings seed = new();
        [SerializeField]
        private GenerationSettings<T> generationTree = new();
        [SerializeField]
        private CollectionSourceSettings<T> collectionSource = new();
        
        private CustomCollectionNestedFieldGeneration<GenerationSettings<T>, T> behaviourTree;
        
        [SerializeField]
        private bool isUnmanaged;

        public bool update;
        
        private ProfilerMarker _generatorMarker = new("CCG:Generate()");
        
        private void OnEnable()
        {
            isUnmanaged = UnsafeUtility.IsUnmanaged(typeof(T));
            
            if (generationTree.fieldTree == null)
                Reset();
            else if (!generationTree.UpdateFieldTree())
                Reset();
            
            (generationTree.fieldTree.children[0] as CustomLeafField<TestManaged, float>).generator =
                new UniformFloatGenerator() { min = 0, max = 1 };
            (generationTree.fieldTree.children[1] as CustomLeafField<TestManaged, int>).generator =
                new UniformIntGenerator() { min = 0, max = 1 };
            (generationTree.fieldTree.children[2] as CustomLeafField<TestManaged, float>).generator =
                new UniformFloatGenerator() { min = 0, max = 1 };
            ((generationTree.fieldTree.children[3] as CustomNestedField<TestManaged, TestManaged.Struct>).children[0] as CustomLeafField<TestManaged.Struct, float>).generator =
                new UniformFloatGenerator() { min = 0, max = 1 };
            ((generationTree.fieldTree.children[3] as CustomNestedField<TestManaged, TestManaged.Struct>).children[1] as CustomLeafField<TestManaged.Struct, int>).generator =
                new UniformIntGenerator() { min = 0, max = 1 };

            behaviourTree = new(generationTree.fieldTree, 0);
        }

        private void Reset()
        {
            seed = new();
            generationTree = new();
            seed.Init();
        }

        public override IEnumerable<T> Generate(ref Random random)
        {
            var col = (NativeArray<T>)collectionSource.provider.GetCollection();
            behaviourTree.GenerateField(col, ref random, default).Complete();
            return col;
        }

        public void Generate()
        {
            var sw = Stopwatch.StartNew();
            _generatorMarker.Begin();
            var col = Generate(ref seed.random);
            _generatorMarker.End();
            sw.Stop();

            var nativeArray = (NativeArray<T>)col;
            for (int i = 0; i < 10; i++)
            {
                Debug.Log(nativeArray[i]);
            }
            nativeArray.Dispose();
            Profiler.enabled = false;
        }

        private void OnValidate()
        {
            if (!update) return;

            collectionSource.provider = new NativeArrayProvider<T>
            {
                allocator = Allocator.Persistent,
                length = 100000,
                options = NativeArrayOptions.ClearMemory
            };
            update = false;
            Generate();
        }
    }
}
