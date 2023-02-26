using System;
using UnityEngine;

namespace PCG.Generation
{
    [Serializable]
    public class CollectionSourceSettings<TCollection>
    {
        [SerializeReference]
        public ObjectAlternative providerAlternative;
        
        [SerializeReference]
        public ICollectionProvider<TCollection> provider;

        public CollectionSourceSettings()
        {
            providerAlternative = new ObjectAlternative(typeof(ICollectionProvider<TCollection>), true);
        }
    }
}