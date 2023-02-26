using UnityEngine;

namespace PCG.Generation
{
    [CreateAssetMenu(menuName = "PCG/Test", fileName = nameof(Test))]
    public class Test : ScriptableObject
    {
        [System.Serializable]
        public struct Struct
        {
            public float fieldA2;
            public int fieldB2;
        }

        public float fieldA1;
        public int fieldB1;

        public float fieldC1;

        public Struct fieldD1;
    }
}
