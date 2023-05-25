
namespace PCG.Generation
{
    public interface IObjectProvider<out T>
    {
        T GetObject();
    }
}
