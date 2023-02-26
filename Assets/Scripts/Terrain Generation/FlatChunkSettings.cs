using UnityEngine;

namespace PCG.Terrain
{
    public abstract class FlatChunkSettings : ScriptableObject
    {
        [HideInInspector] public bool dirty;

        private void OnValidate()
        {
            dirty = true;
        }

        public abstract void Generate(FlatChunk chunk);
    }
}