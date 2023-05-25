using PCG.Generation;
using UnityEngine;

namespace PCG.Terrain.Scripts
{
    [CreateAssetMenu(menuName = "Sample/" + nameof(CustomStarGenerator), fileName = nameof(CustomStarGenerator))]
    public class CustomStarGenerator : CustomCollectionGenerator<SampleStar>
    {
        
    }
}