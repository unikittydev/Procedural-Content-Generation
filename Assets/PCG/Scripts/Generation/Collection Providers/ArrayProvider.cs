using System;
using System.Collections.Generic;

namespace PCG.Generation
{
    [DisplayName("Array")]
    [Serializable]
    public class ArrayProvider<T> : ICollectionProvider<T>
    {
        public int length;

        private T[] array;

        public int Length => array.Length;

        public void Create()
        {
            array = new T[length];
        }

        public IEnumerable<T> GetCollection()
        {
            return array;
        }
    }
}
