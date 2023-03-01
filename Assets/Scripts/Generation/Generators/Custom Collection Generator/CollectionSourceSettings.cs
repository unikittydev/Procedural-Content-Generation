using System;
using UnityEngine;

namespace PCG.Generation
{
    [Serializable]
    public class CollectionSourceSettings<T>
    {
        [SerializeReference]
        public ObjectAlternative providerAlternative;
        
        [SerializeReference]
        public ICollectionProvider<T> provider;

        public CollectionSourceSettings()
        {
            providerAlternative = new ObjectAlternative(typeof(ICollectionProvider<T>), true);
        }
    }
}