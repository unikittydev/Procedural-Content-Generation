using System.Collections.Generic;
using PCG.Terrain;
using UnityEngine;

namespace PCG.Terrain
{
    [System.Serializable]
    public abstract class TextureCreator
    {
        private delegate Texture2D TextureCreatorDelegate(in Chunk2D chunk); 

        private readonly Dictionary<ChunkMeshTextureFormat, TextureCreatorDelegate> creators;
        
        [SerializeField] private ChunkMeshTextureFormat format;

        protected TextureCreator()
        {
            creators = new()
            {
                { ChunkMeshTextureFormat.R32, To_R32_Texture },
                { ChunkMeshTextureFormat.R32G32B32, To_R32G32B32_Texture }
            };
        }
        
        public Texture2D ToTexture2D(in Chunk2D chunk)
        {
            return creators[format](chunk);
        }

        protected abstract Texture2D To_R32_Texture(in Chunk2D chunk);

        protected abstract Texture2D To_R32G32B32_Texture(in Chunk2D chunk);
    }
}