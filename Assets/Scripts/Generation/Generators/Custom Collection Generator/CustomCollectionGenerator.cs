using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    public class CustomCollectionGenerator<T> : GeneratorAsset<T>, IGenerateButtonCallback where T : new()
    {
        [SerializeField]
        private SeedSettings seed = new();
        // ???
        [SerializeField]
        private GenerationSettings<T> generationTree = new();
        [SerializeField]
        private CollectionSourceSettings<T> collectionSource = new();

        private bool isUnmanaged;
        
        private void OnEnable()
        {
            isUnmanaged = UnsafeUtility.IsUnmanaged(typeof(T));
            
            if (generationTree.fieldTree == null)
                Reset();
            else if (!generationTree.UpdateFieldTree())
                Reset();
        }

        private void Reset()
        {
            seed = new();
            generationTree = new();
            seed.Init();
        }

        public override T Generate(ref Random random)
        {
            throw new NotImplementedException();
        }

        public void Generate()
        {
            throw new NotImplementedException();
        }
    }
}
