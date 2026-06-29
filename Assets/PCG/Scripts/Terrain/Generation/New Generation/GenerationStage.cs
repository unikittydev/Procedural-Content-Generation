using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PCG.Terrain
{
    [CreateAssetMenu(menuName = "PCG/Test Stage")]
    public abstract class GenerationStage : ScriptableObject
    {
        public Map2DPool mapPool { get; set; }

        [SerializeField] private ChunkState chunkStateOnStart = ChunkState.Unloaded;
        
        [SerializeReference] private GenerationParameter[] inParameters;
        [SerializeReference] private GenerationParameter[] outParameters;

        private Dictionary<string, IChunk2D> inMaps = new();
        private Dictionary<string, IChunk2D> outMaps = new();

        private GenerationStage previousStage;

        protected Chunk2D chunk;
        
        internal void OnBeforeGenerate(GenerationStage previousStage, Chunk2D chunk)
        {
            this.previousStage = previousStage;
            this.chunk = chunk;
            chunk.state = chunkStateOnStart;
            
            if (previousStage == null)
                AddMaps(inParameters, inMaps);
            else
                AddInMapsFromPreviousOutput();
            
            AddMaps(outParameters, outMaps);
        }

        public abstract IEnumerator OnGenerate();
        
        internal void OnAfterGenerate()
        {
            if (previousStage == null)
                ReleaseMaps(inParameters, inMaps);
            ReleaseMaps(outParameters, outMaps);
        }

        private void OnValidate()
        {
            for (int i = 0; i < inParameters.Length; i++) inParameters[i] ??= new GenerationParameter();
            for (int i = 0; i < outParameters.Length; i++) outParameters[i] ??= new GenerationParameter();
        }

        private void AddMaps(GenerationParameter[] parameters, Dictionary<string, IChunk2D> maps)
        {
            foreach (GenerationParameter param in parameters)
            {
                var type = param.parameterType.GetChoiceType();
                var map = mapPool.Get(type, chunk.lodLevel);
                inMaps.Add(param.name, map);
            }
        }

        private void AddInMapsFromPreviousOutput()
        {
            inMaps = previousStage.outMaps;
        }
        
        private void ReleaseMaps(GenerationParameter[] parameters, Dictionary<string, IChunk2D> maps)
        {
            foreach (GenerationParameter param in parameters)
            {
                var type = param.parameterType.GetChoiceType();
                var map = maps[param.name];
                mapPool.Add(type, chunk.lodLevel, map);
            }
            maps.Clear();
        }

        protected TChunk GetInputMap<TChunk>(string name) where TChunk : IChunk2D
        {
            return (TChunk)inMaps[name];
        }
        
        protected TChunk GetOutputMap<TChunk>(string name) where TChunk : IChunk2D
        {
            return (TChunk)outMaps[name];
        }
    }
}
