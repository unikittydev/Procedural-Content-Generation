using System;
using UnityEngine;

namespace PCG.Generation
{
    public abstract class SourceSettings { }
    
    [Serializable]
    public abstract class SourceSettings<TProvider> : SourceSettings
    {
        [SerializeReference]
        public ObjectAlternative providerAlternative;
        
        [SerializeReference]
        public TProvider provider;
        
        protected SourceSettings()
        {
            providerAlternative = new ObjectAlternative(typeof(TProvider), true);
        }
    }
}