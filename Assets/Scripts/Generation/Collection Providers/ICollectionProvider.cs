
namespace PCG.Generation
{
    public interface ICollectionProvider<out TCollection>
    {
        TCollection GetCollection();
    }
}
