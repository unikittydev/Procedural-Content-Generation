using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public interface IGenerateButtonCallback
    {
        public void Generate();
    }

    public class CustomObjectGenerator<T> : GeneratorAsset<T>, IGenerateButtonCallback where T : new()
    {
        [SerializeField] private SeedSettings seed = new();

        [SerializeField] private GenerationSettings<T> generationTree = new();

        [SerializeField] private SourceSettings<T> source = new();
        
        [SerializeField] private int N;

        [SerializeField] private string typeName;

        private ProfilerMarker _generatorMarker = new("COG:Generate()");

        private void OnEnable()
        {
            TypeMapper.AddType(typeof(T));
            typeName = typeof(T).AssemblyQualifiedName;
            
            if (generationTree.fieldTree == null)
                Reset();
            else if (!generationTree.UpdateFieldTree())
                Reset();
        }
        
        private void Reset()
        {
            seed = new SeedSettings();
            generationTree = new GenerationSettings<T>();
            source = new SourceSettings<T>();
            seed.Init();
        }

        public override T Generate(ref Random random)
        {
            generationTree._currentObject = source.provider.GetObject();
            generationTree.fieldTree.GenerateField(ref generationTree, ref random);
            return generationTree._currentObject;
        }

        public void Generate()
        {
            _generatorMarker.Begin();
            for (int i = 0; i < N; i++)
                generationTree._currentObject = Generate(ref seed.random);
            _generatorMarker.End();
            Profiler.enabled = false;
        }
    }
}
