using System;
using Unity.Collections;

namespace PCG.Generation
{
    [Serializable]
    public struct NativeArrayProvider<T> : ICollectionProvider<NativeArray<T>> where T : unmanaged
    {
        public int length;
        public Allocator allocator;
        public NativeArrayOptions options;

        public NativeArray<T> GetCollection()
        {
            return new NativeArray<T>(length, allocator, options);
        }
    }
}
