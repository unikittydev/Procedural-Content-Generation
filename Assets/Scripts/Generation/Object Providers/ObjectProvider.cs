using System;

namespace PCG.Generation
{
    [Serializable]
    public class ObjectProvider<T> : IObjectProvider<T> where T : new()
    {
        public T GetObject()
        {
            return new T();
        }
    }
}
