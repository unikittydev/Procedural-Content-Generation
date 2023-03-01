using UnityEngine;

namespace PCG.Generation
{
    [CreateAssetMenu(fileName = nameof(TestCollectionGenerator), menuName = "PCG/" + nameof(TestCollectionGenerator))]
    public class TestCollectionGenerator : CustomCollectionGenerator<TestManaged>
    {
    }
}
