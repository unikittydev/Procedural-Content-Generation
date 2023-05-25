using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace PCG.Terrain
{
    [System.Serializable]
    public class NormalsTextureCreator : TextureCreator
    {
        protected override Texture2D To_R32_Texture(in Chunk2D chunk)
        {
            throw new System.NotImplementedException();
        }

        protected override Texture2D To_R32G32B32_Texture(in Chunk2D chunk)
        {
            var texture = new Texture2D(chunk.resolution.x, chunk.resolution.y, GraphicsFormat.R32G32B32_SFloat,
                TextureCreationFlags.DontInitializePixels);

            texture.SetPixelData(chunk.GetContent<ChunkNormals2D>().content, 0, 0);
            texture.Apply();

            return texture;
        }
    }
}