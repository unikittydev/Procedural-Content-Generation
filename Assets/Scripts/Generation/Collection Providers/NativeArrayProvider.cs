using System;
using System.Collections.Generic;
using Unity.Collections;

namespace PCG.Generation
{
    [Serializable]
    public struct NativeArrayProvider<T> : ICollectionProvider<T> where T : unmanaged
    {
        public int length;
        public Allocator allocator;
        public NativeArrayOptions options;

        public IEnumerable<T> GetCollection()
        {
            return new NativeArray<T>(length, allocator, options);
        }
    }
}
