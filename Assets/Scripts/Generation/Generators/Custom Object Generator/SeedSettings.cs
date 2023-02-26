using System;
using Random = Unity.Mathematics.Random;

namespace PCG.Generation
{
    [Serializable]
    public class SeedSettings
    {
        public uint initialSeed;
        public Random random;

        public void Init()
        {
            initialSeed = unchecked((uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
            random = new Random(initialSeed);
        }
    }
}