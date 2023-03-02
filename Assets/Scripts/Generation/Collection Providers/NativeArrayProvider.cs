using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace PCG.Generation
{
    [Serializable]
    public struct NativeArrayProvider<T> : INativeCollectionProvider<T> where T : unmanaged
    {
        public int length;
        public Allocator allocator;
        public NativeArrayOptions options;

        private NativeArray<T> array;
        private IEnumerable<T> enumerable;

        public int Length => array.Length;
        
        public void Create()
        {
            array = new NativeArray<T>(length, allocator, options);
            enumerable = array;
        }
        
        public unsafe void* GetUnsafePtr() => array.GetUnsafePtr();

        public IEnumerable<T> GetCollection() => enumerable;
    }
}
