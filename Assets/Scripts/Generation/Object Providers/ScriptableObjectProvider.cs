using UnityEngine;

namespace PCG.Generation
{
    [System.Serializable]
    public class ScriptableObjectProvider<T> : IObjectProvider<T> where T : ScriptableObject
    {
        public T GetObject()
        {
            return ScriptableObject.CreateInstance<T>();
        }
    }
}
