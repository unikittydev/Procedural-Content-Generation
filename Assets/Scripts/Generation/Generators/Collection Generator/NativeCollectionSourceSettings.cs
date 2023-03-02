using System;

namespace PCG.Generation
{
    [Serializable]
    public class NativeCollectionSourceSettings<T> : SourceSettings<INativeCollectionProvider<T>> where T : unmanaged { }
}