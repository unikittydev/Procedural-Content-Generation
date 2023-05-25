using System;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    [Serializable]
    public struct SampleStar
    {
        public float3 position;

        public float mass;
        [HideInInspector]
        public float temperature;
        [HideInInspector]
        public float radius;
        [HideInInspector]
        public float luminocity;
        [HideInInspector]
        public Color color;
    }
}
