using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace PCG.Terrain
{
    [System.Serializable]
    public class NormalsTextureCreator : TextureCreator
    {
        protected override Texture2D To_R32_Texture(in FlatChunk chunk)
        {
            throw new System.NotImplementedException();
        }

        protected override Texture2D To_R32G32B32_Texture(in FlatChunk chunk)
        {
            //throw new System.NotImplementedException();
            
            var texture = new Texture2D(chunk.world.chunkSize.x, chunk.world.chunkSize.y, GraphicsFormat.R32G32B32_SFloat,
                TextureCreationFlags.DontInitializePixels);

            texture.SetPixelData(chunk.normals, 0, 0);
            texture.Apply();

            return texture;
        }
    }
}