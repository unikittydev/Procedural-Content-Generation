using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public class ObjectGenerator<T> : CustomGenerator<ObjectSourceSettings<T>, T>, IGenerator<T> where T : new()
    {
        private ObjectNestedFieldGenerator<GenerationSettings<T>, T> behaviourTree;

        private ProfilerMarker _generatorMarker = new("COG:Generate()");

        protected override void OnEnable()
        {
            base.OnEnable();
            behaviourTree ??= new(generationTree.fieldTree);
        }

        protected override void Reset()
        {
            base.Reset();
            behaviourTree ??= new(generationTree.fieldTree);
        }

        public T Generate(ref Random random)
        {
            generationTree.currentObject = source.provider.GetObject();
            behaviourTree.GenerateField(ref generationTree, ref random);
            return generationTree.currentObject;
        }

        public override void Generate()
        {
            _generatorMarker.Begin();
            generationTree.currentObject = Generate(ref seed.random);
            _generatorMarker.End();
            Profiler.enabled = false;
        }
    }
}
