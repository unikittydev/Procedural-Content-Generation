using UnityEngine;

namespace PCG.Generation
{
    [DisplayName("New ScriptableObject")]
    [System.Serializable]
    public class ScriptableObjectProvider<T> : IObjectProvider<T> where T : ScriptableObject
    {
        public T GetObject()
        {
            return ScriptableObject.CreateInstance<T>();
        }
    }
}
