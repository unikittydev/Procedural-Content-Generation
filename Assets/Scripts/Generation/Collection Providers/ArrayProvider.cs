using System;
using System.Collections.Generic;

namespace PCG.Generation
{
    [Serializable]
    public class ArrayProvider<T> : ICollectionProvider<T>
    {
        public int length;
        
        public IEnumerable<T> GetCollection()
        {
            return new T[length];
        }
    }
}
