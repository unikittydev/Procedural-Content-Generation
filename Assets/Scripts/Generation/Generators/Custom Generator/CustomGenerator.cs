using Unity.Profiling;
using UnityEngine;

namespace PCG.Generation
{
    public interface IGenerateButtonCallback
    {
        public void Generate();
    }

    public abstract class CustomGenerator<TSourceSettings, T> : ScriptableObject
        where TSourceSettings : SourceSettings, new()
        where T : new()
    {
        [SerializeField] private SeedSettings seed = new();
        
        [SerializeField] private GenerationSettings<T> generationTree = new();

        [SerializeField] private TSourceSettings source;
        
        private ProfilerMarker _generatorMarker = new("CCG:Generate()");
        
        protected virtual void OnEnable()
        {
            TypeMapper.AddType(typeof(T));
            
            if (generationTree.fieldTree == null)
                Reset();
            else if (!generationTree.UpdateFieldTree())
                Reset();
        }

        protected virtual void Reset()
        {
            seed = new SeedSettings();
            generationTree = new GenerationSettings<T>();
            source = new TSourceSettings();
            seed.Init();
        }
    }
}