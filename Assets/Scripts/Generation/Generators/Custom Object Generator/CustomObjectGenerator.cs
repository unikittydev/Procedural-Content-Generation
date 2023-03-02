using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public class CustomObjectGenerator<T> : GeneratorAsset<T>, IGenerateButtonCallback where T : new()
    {
        [SerializeField] private SeedSettings seed = new();

        [SerializeField] private GenerationSettings<T> generationTree = new();

        [SerializeField] private ObjectSourceSettings<T> objectSource = new();
        
        private CustomObjectNestedFieldGeneration<GenerationSettings<T>, T> behaviourTree;

        private ProfilerMarker _generatorMarker = new("COG:Generate()");

        private void OnEnable()
        {
            TypeMapper.AddType(typeof(T));
            
            if (generationTree.fieldTree == null)
                Reset();
            else if (!generationTree.UpdateFieldTree())
                Reset();

            behaviourTree = new(generationTree.fieldTree);
        }
        
        private void Reset()
        {
            seed = new SeedSettings();
            generationTree = new GenerationSettings<T>();
            objectSource = new ObjectSourceSettings<T>();
            seed.Init();
        }

        public override T Generate(ref Random random)
        {
            generationTree.currentObject = objectSource.provider.GetObject();
            behaviourTree.GenerateField(ref generationTree, ref random);
            return generationTree.currentObject;
        }

        public void Generate()
        {
            _generatorMarker.Begin();
            generationTree.currentObject = Generate(ref seed.random);
            _generatorMarker.End();
            Profiler.enabled = false;
        }
    }
}
