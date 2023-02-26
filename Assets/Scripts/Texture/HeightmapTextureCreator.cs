using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace PCG.Terrain
{
    [System.Serializable]
    public class HeightmapTextureCreator : TextureCreator
    {
        protected override Texture2D To_R32_Texture(in FlatChunk chunk)
        {
            var texture = new Texture2D(chunk.world.chunkSize.x, chunk.world.chunkSize.y, GraphicsFormat.R32_SFloat,
                TextureCreationFlags.DontInitializePixels);

            texture.SetPixelData(chunk.density, 0, 0);
            texture.Apply();

            return texture;
        }

        protected override Texture2D To_R32G32B32_Texture(in FlatChunk chunk)
        {
            var texture = new Texture2D(chunk.world.chunkSize.x, chunk.world.chunkSize.y, GraphicsFormat.R32G32B32_SFloat,
                TextureCreationFlags.DontInitializePixels);

            NativeArray<float> colors = texture.GetPixelData<float>(0);
            
            unsafe
            {
                float* source = (float*)chunk.density.GetUnsafeReadOnlyPtr();
                float* dest = (float*)colors.GetUnsafePtr();

                UnsafeUtility.MemCpyStride(dest, sizeof(float) * 3, source, sizeof(float) * 1, sizeof(float),
                    chunk.density.Length);
                UnsafeUtility.MemCpyStride(dest + 1, sizeof(float) * 3, source, sizeof(float) * 1, sizeof(float),
                    chunk.density.Length);
                UnsafeUtility.MemCpyStride(dest + 2, sizeof(float) * 3, source, sizeof(float) * 1, sizeof(float),
                    chunk.density.Length);
            }

            texture.Apply();

            return texture;
        }
    }
}