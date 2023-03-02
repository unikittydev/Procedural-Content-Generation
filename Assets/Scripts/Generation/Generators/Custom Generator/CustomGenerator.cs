using System;
using UnityEngine;

namespace PCG.Generation
{
    public interface IGenerateButtonCallback
    {
        public void Generate();
    }

    public abstract class CustomGenerator<TSourceSettings, T> : ScriptableObject, IGenerateButtonCallback
        where TSourceSettings : SourceSettings, new()
        where T : new()
    {
        protected virtual bool allowManaged => true;
        
        [SerializeField] protected SeedSettings seed;
        
        [SerializeField] protected GenerationSettings<T> generationTree;

        [SerializeReference] protected TSourceSettings source;
        
        protected virtual void OnEnable()
        {
            TypeMapper.AddType(typeof(T));
            
            if (generationTree?.fieldTree == null)
                Reset();
            else if (!generationTree.UpdateFieldTree())
                Reset();
        }

        protected virtual void Reset()
        {
            seed = new SeedSettings();
            generationTree = new GenerationSettings<T>(allowManaged);
            source = new TSourceSettings();
            seed.Init();
        }

        public abstract void Generate();
    }
}