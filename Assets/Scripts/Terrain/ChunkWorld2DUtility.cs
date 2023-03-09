using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    public static class ChunkWorld2DUtility
    {
        public static int2 GetChunkPosition(this ChunkWorld2D world, Vector3 localPosition)
        {
            Vector3 scaledPosition = localPosition / world.chunkSize;
            return new int2((int)math.floor(scaledPosition.x),(int)math.floor(scaledPosition.z));
        }
    }
}