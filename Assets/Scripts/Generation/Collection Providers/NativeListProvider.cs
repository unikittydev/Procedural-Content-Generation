using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace PCG.Generation
{
    [Serializable]
    public class NativeListProvider<T> : INativeCollectionProvider<T> where T : unmanaged
    {
        public int length;
        public Allocator allocator = Allocator.Temp;

        private NativeList<T> list;
        private IEnumerable<T> enumerable;

        public int Length => list.Length;
        public void Create()
        {
            var allocatorHandle = AllocatorManager.ConvertToAllocatorHandle(allocator);
            list =  new NativeList<T>(length, allocatorHandle);
            list.Length = length;
            enumerable = list;
        }

        public IEnumerable<T> GetCollection() => enumerable;

        public unsafe void* GetUnsafePtr() => list.GetUnsafePtr();
    }
}