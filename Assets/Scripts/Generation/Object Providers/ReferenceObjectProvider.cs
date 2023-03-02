using UnityEngine;

namespace PCG.Generation
{
    [DisplayName("Reference")]
    [System.Serializable]
    public class ReferenceObjectProvider<T> : IObjectProvider<T>
    {
        [SerializeReference]
        private T reference;
        
        public T GetObject()
        {
            return reference;
        }
    }
}
