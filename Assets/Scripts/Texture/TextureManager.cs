using UnityEngine;

namespace PCG.Terrain
{
    public class TextureManager : MonoBehaviour
    {
        [SerializeField, InspectorName("Heightmap Settings")] private HeightmapTextureCreator _hmTextureCreator = new HeightmapTextureCreator();
        
        public HeightmapTextureCreator heightmapTextureCreator => _hmTextureCreator;

        [SerializeField, InspectorName("Normalmap Settings")]
        private NormalsTextureCreator _nTextureCreator = new NormalsTextureCreator();

        public NormalsTextureCreator normalMapTextureCreator => _nTextureCreator;
    }
}