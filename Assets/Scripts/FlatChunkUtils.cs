using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.TerrainGeneration
{
    /// <summary>
    /// Extension methods assotiated with FlatChunk.
    /// </summary>
    public static class FlatChunkUtils
    {
        /// <summary>
        /// Gets index for 1D array from 2D position for given chunk.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndex(in int2 chunkSize, in int2 pos)
        {
            return GetIndex(chunkSize, pos.x, pos.y);
        }

        /// <summary>
        /// Gets index for 1D array from 2D position for given chunk.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndex(in int2 chunkSize, in int x, in int y)
        {
            return chunkSize.x * y + x;
        }
        
        /// <summary>
        /// Gets 2D position from index for given chunk.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 GetPosition(in int2 chunkSize, in int index)
        {
            return new int2(index % chunkSize.x, index / chunkSize.x);
        }

        /// <summary>
        /// Converts position within chunk to position within flat world.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 LocalToWorldPosition(in int2 chunkPosition, in int2 chunkSize, in int2 pos)
        {
            return chunkPosition * chunkSize + pos;
        }
        
        /// <summary>
        /// Converts position within chunk to position within flat world.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 LocalToWorldPosition(in int2 chunkPosition, in int2 chunkSize, in int x, in int y)
        {
            return LocalToWorldPosition(chunkPosition, chunkSize, new int2(x, y));
        }

        /// <summary>
        /// Converts position within chunk to transform local position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LocalToTransformLocalPosition(in int2 pos, in float value, in Vector3 terrainScale)
        {
            return LocalToTransformLocalPosition(pos.x, pos.y, value, terrainScale);
        }
        
        /// <summary>
        /// Converts position within chunk to transform local position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LocalToTransformLocalPosition(in int x, in int y, in float value, in Vector3 terrainScale)
        {
            return Vector3.Scale(new Vector3(x, value,y), terrainScale);
        }
        
        /// <summary>
        /// Converts position within chunk to transform world position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LocalToTransformWorldPosition(in Vector3 transformWorldPosition, in int2 pos, in float value, in Vector3 terrainScale)
        {
            return LocalToTransformWorldPosition(transformWorldPosition, pos.x, pos.y, value, terrainScale);
        }
        
        /// <summary>
        /// Converts position within chunk to transform world position.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LocalToTransformWorldPosition(in Vector3 transformWorldPosition, in int x, in int y, in float value, in Vector3 terrainScale)
        {
            return transformWorldPosition + LocalToTransformLocalPosition(x, y, value, terrainScale);
        }
    }
}