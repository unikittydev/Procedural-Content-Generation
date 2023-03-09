﻿using UnityEngine;

namespace PCG.Terrain
{
    [DisallowMultipleComponent]
    public class ChunkLoader2D : MonoBehaviour
    {
        [field: SerializeField]
        public int viewRadiusInChunks { get; private set; }
    }
}