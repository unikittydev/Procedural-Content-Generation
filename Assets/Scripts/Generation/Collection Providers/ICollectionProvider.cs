
using System.Collections.Generic;

namespace PCG.Generation
{
    public interface ICollectionProvider<out T>
    {
        IEnumerable<T> GetCollection();
    }
}
