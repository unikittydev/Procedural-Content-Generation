using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

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
        private NativeCollectionSourceSettings<T> nativeCollectionSource = new();
        
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
            behaviourTree.GenerateField(nativeCollectionSource.provider, ref random, default).Complete();
            return nativeCollectionSource.provider.GetCollection();
        }

        public void Generate()
        {
            _generatorMarker.Begin();
            Generate(ref seed.random);
            _generatorMarker.End();
            Profiler.enabled = false;
        }

        private void OnValidate()
        {
            if (!update) return;

            nativeCollectionSource.provider = new NativeArrayProvider<T>
            {
                allocator = Allocator.Persistent,
                length = 100000,
                options = NativeArrayOptions.UninitializedMemory
            };
            nativeCollectionSource.provider.Create();
            update = false;
            Generate();
        }
    }
}
