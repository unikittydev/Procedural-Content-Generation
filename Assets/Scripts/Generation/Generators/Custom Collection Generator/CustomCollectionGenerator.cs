using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.Profiling;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public class CustomCollectionGenerator<T> : CustomGenerator<NativeCollectionSourceSettings<T>, T>, IGenerator<IEnumerable<T>> where T : unmanaged
    {
        private CustomCollectionNestedFieldGeneration<GenerationSettings<T>, T> behaviourTree;
        
        private ProfilerMarker _generatorMarker = new("CCG:Generate()");
        
        protected override void OnEnable()
        {
            base.OnEnable();
            behaviourTree = new(generationTree.fieldTree, 0);
        }

        public IEnumerable<T> Generate(ref Random random)
        {
            source.provider.Create();
            behaviourTree.GenerateField(source.provider, ref random, default).Complete();
            return source.provider.GetCollection();
        }

        public override void Generate()
        {
            _generatorMarker.Begin();
            Generate(ref seed.random);
            _generatorMarker.End();
            Profiler.enabled = false;
        }
    }
}
