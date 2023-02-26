using System;
using UnityEngine;

namespace PCG.Generation
{
    [Serializable]
    public class SourceSettings<T>
    {
        [SerializeReference]
        public ObjectAlternative providerAlternative;

        [SerializeReference]
        public IObjectProvider<T> provider;
        
        public SourceSettings()
        {
            providerAlternative = new ObjectAlternative(typeof(IObjectProvider<T>), true);
        }
    }
}