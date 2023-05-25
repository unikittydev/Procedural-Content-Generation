using UnityEngine;

namespace PCG.Generation
{
    [DisplayName("Instantiate prefab")]
    [System.Serializable]
    public class MonoBehaviourProvider<T> : IObjectProvider<T> where T : MonoBehaviour
    {
        [SerializeReference] private T prefab;
        
        public T GetObject()
        {
            return Object.Instantiate(prefab);
        }
    }
}
