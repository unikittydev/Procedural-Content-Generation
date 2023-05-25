
namespace PCG.Generation
{
    public interface INativeCollectionProvider<out T> : ICollectionProvider<T> where T : unmanaged
    {
        unsafe void* GetUnsafePtr();
    }
}