using System.Collections.Generic;
using UnityEngine;

namespace PCG.Terrain
{
    public class ObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private int capacity;

        [SerializeField]
        private T prefab;

        private Queue<T> pool;

        private Transform disabledChild;

        private void Awake()
        {
            disabledChild = new GameObject("Pool").transform;
            disabledChild.gameObject.SetActive(false);
        }

        private void Start()
        {
            pool = new Queue<T>(capacity);

            for (int i = 0; i < capacity; i++)
                AddInstance();
        }

        private void AddInstance()
        {
            var chunk = Instantiate(prefab, disabledChild);
            chunk.name += chunk.GetInstanceID();
            pool.Enqueue(chunk);
        }
        
        public void Add(T chunk)
        {
            chunk.transform.SetParent(disabledChild);
            pool.Enqueue(chunk);
        }
        
        public T Get(Transform parent)
        {
            if (pool.Count == 0)
                AddInstance();
            T chunk = pool.Dequeue();
            chunk.transform.SetParent(parent);
            return chunk;
        }

        public T Get() => Get(null);
    }
}