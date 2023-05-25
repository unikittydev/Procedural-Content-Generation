using System;

namespace PCG.Generation
{
    
    [DisplayName("New instance")]
    [Serializable]
    public class ObjectProvider<T> : IObjectProvider<T> where T : new()
    {
        public T GetObject()
        {
            return new T();
        }
    }
}
