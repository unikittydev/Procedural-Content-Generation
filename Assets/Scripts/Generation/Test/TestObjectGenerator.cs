using UnityEngine;

namespace PCG.Generation
{
    [CreateAssetMenu(menuName = "PCG/" + nameof(TestObjectGenerator), fileName = nameof(TestObjectGenerator))]
    public class TestObjectGenerator : ObjectGenerator<Test>
    {
    }
}
