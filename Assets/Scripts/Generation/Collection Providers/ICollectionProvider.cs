
using System.Collections.Generic;

namespace PCG.Generation
{
    public interface ICollectionProvider<out T>
    {
        int Length { get; }
        
        void Create();
        
        IEnumerable<T> GetCollection();
    }
}
