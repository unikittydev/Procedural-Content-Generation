
namespace PCG.Generation
{
    public struct TestManaged
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

        public override string ToString()
        {
            return $"({fieldA1:0.00}, {fieldB1}, {fieldC1:0.00}, ({fieldD1.fieldA2:0.00}, {fieldD1.fieldB2}))";
        }
    }
}
