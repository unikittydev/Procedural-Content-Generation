using System;
using PCG.Generation;
using UnityEngine;

namespace PCG.Terrain
{
    [Serializable]
    public class GenerationParameter
    {
        [field: SerializeField]
        public string name { get; private set; }

        [field: SerializeReference]
        public ObjectAlternative parameterType { get; set; } = new (typeof(IChunk2D), Type.EmptyTypes, false, false, false);
    }
}