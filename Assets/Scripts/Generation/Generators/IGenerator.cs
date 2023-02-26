using Unity.Mathematics;

namespace PCG.Generation
{
    public interface IGenerator<out T>
    {
        T Generate(ref Random random);
    }
}
