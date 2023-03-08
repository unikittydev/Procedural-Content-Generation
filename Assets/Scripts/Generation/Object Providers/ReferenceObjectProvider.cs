using UnityEngine;

namespace PCG.Generation
{
    [DisplayName("Reference")]
    [System.Serializable]
    public class ReferenceObjectProvider<T> : IObjectProvider<T> where T : Object
    {
        //[SerializeReference]
        [SerializeField]
        private T reference;
        
        public T GetObject()
        {
            return reference;
        }
    }
}
