using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    public static class ChunkWorld2DUtility
    {
        public static int2 GetChunkPosition(this ChunkWorld2D world, Vector3 localPosition)
        {
            Vector3 scaledPosition = localPosition / world.chunkWorldSize;
            return new int2((int)math.floor(scaledPosition.x),(int)math.floor(scaledPosition.z));
        }

        public static Vector3 GetChunkCenterBottomWorldPosition(this ChunkWorld2D world, int2 pos)
        {
            return new Vector3(pos.x + .5f, 0f, pos.y + .5f) * world.chunkWorldSize;
        }
    }
}