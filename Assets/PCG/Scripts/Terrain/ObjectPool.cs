using System.Collections.Generic;
using UnityEngine;

namespace PCG.Terrain
{
    public abstract class ObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private int capacity;

        [SerializeField]
        protected T prefab;

        private Queue<T> pool;

        private Transform disabledChild;

        private void Awake()
        {
            disabledChild = new GameObject($"Pool of {typeof(T).Name}").transform;
            disabledChild.gameObject.SetActive(false);
        }

        private void Start()
        {
            InitPrefab();
            
            pool = new Queue<T>(capacity);

            AddInstances(capacity);
        }

        protected virtual void InitPrefab() { }

        private void AddInstances(int count)
        {
            for (int i = 0; i < count; i++)
                AddInstance();
        }
        
        protected virtual T AddInstance()
        {
            var instance = Instantiate(prefab, disabledChild);
            instance.name += instance.GetInstanceID();
            pool.Enqueue(instance);
            return instance;
        }
        
        public void Add(T instance)
        {
            instance.transform.SetParent(disabledChild);
            pool.Enqueue(instance);
        }
        
        public T Get(Transform parent)
        {
            if (pool.Count == 0)
                AddInstances(capacity);
            T instance = pool.Dequeue();
            instance.transform.SetParent(parent);
            return instance;
        }

        public T Get() => Get(null);
    }
}