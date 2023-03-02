using System;

namespace PCG.Generation
{
    [Serializable]
    public class ObjectSourceSettings<T> : SourceSettings<IObjectProvider<T>> { }
}