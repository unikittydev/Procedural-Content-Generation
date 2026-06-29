using System.Collections;
using PCG.Terrain;
using Unity.Mathematics;
using UnityEngine;

namespace Sample
{
    public class SampleChunk2DLerp : MonoBehaviour
    {
        [SerializeField] private SampleChunk2DNoiseGenerator from, to, target;
        [SerializeField] private Material fromMat, toMat, targetMat;
        [SerializeField] private float delay;
        [SerializeField] private float time;

        [SerializeField] private SampleChunk2DLerp next;

        private IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(delay);
            Lerp();
            yield return new WaitForSecondsRealtime(time);
            if (next)
                next.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        public void Lerp()
        {
            if (from && to && target)
                StartCoroutine(LerpSettingsCoroutine());
            if (fromMat && toMat && targetMat)
                StartCoroutine(LerpMaterialCoroutine(fromMat, toMat, targetMat, time));
        }

        private IEnumerator LerpSettingsCoroutine()
        {
            float counter = 0f, t = 0f;

            while (t < 1f)
            {
                t = Mathf.Clamp01(counter / time);

                target.settings.frequency = Mathf.Lerp(from.settings.frequency, to.settings.frequency, t);
                target.settings.persistence = Mathf.Lerp(from.settings.persistence, to.settings.persistence, t);
                target.settings.lacunarity = Mathf.Lerp(from.settings.lacunarity, to.settings.lacunarity, t);
                target.settings.octaves = (int)Mathf.Lerp(from.settings.octaves, to.settings.octaves, t);
                target.settings.redistributionPower = Mathf.Lerp(from.settings.redistributionPower, to.settings.redistributionPower, t);
                target.settings.erosionPower = Mathf.Lerp(from.settings.erosionPower, to.settings.erosionPower, t);
                target.settings.warpingOffset = math.lerp(from.settings.warpingOffset, to.settings.warpingOffset, t);
                target.settings.warpingStrenth = Mathf.Lerp(from.settings.warpingStrenth, to.settings.warpingStrenth, t);

                counter += Time.deltaTime;

                ChunkWorld2D.dirty = true;
                
                yield return null;
            }
        }

        private IEnumerator LerpMaterialCoroutine(Material from, Material to, Material target, float time)
        {
            int _Height_cutoff = Shader.PropertyToID("_Height_cutoff");
            int _Slope_cutoff = Shader.PropertyToID("_Slope_cutoff");
            int _World_height = Shader.PropertyToID("_World_height");
            int _Invert_sand = Shader.PropertyToID("_Invert_sand");
            
            int _Vegetation = Shader.PropertyToID("_Vegetation");
            int _Sand = Shader.PropertyToID("_Sand");
            int _Rock = Shader.PropertyToID("_Rock");
            
            int _Vegetation_smoothness = Shader.PropertyToID("_Vegetation_smoothness");
            int _Sand_smoothness = Shader.PropertyToID("_Sand_smoothness");
            int _rock_smoothness = Shader.PropertyToID("_rock_smoothness");
            
            float counter = 0f, t = 0f;

            while (t < 1f)
            {
                t = Mathf.Clamp01(counter / time);
                
                target.SetFloat(_Height_cutoff, Mathf.Lerp(from.GetFloat(_Height_cutoff), to.GetFloat(_Height_cutoff), t));
                target.SetFloat(_Slope_cutoff, Mathf.Lerp(from.GetFloat(_Slope_cutoff), to.GetFloat(_Slope_cutoff), t));
                target.SetFloat(_World_height, Mathf.Lerp(from.GetFloat(_World_height), to.GetFloat(_World_height), t));
                target.SetFloat(_Invert_sand, Mathf.Lerp(from.GetFloat(_Invert_sand), to.GetFloat(_Invert_sand), t));
                
                target.SetColor(_Vegetation, Color.Lerp(from.GetColor(_Vegetation), to.GetColor(_Vegetation), t));
                target.SetColor(_Sand, Color.Lerp(from.GetColor(_Sand), to.GetColor(_Sand), t));
                target.SetColor(_Rock, Color.Lerp(from.GetColor(_Rock), to.GetColor(_Rock), t));
                
                target.SetFloat(_Vegetation_smoothness, Mathf.Lerp(from.GetFloat(_Vegetation_smoothness), to.GetFloat(_Vegetation_smoothness), t));
                target.SetFloat(_Sand_smoothness, Mathf.Lerp(from.GetFloat(_Sand_smoothness), to.GetFloat(_Sand_smoothness), t));
                target.SetFloat(_rock_smoothness, Mathf.Lerp(from.GetFloat(_rock_smoothness), to.GetFloat(_rock_smoothness), t));

                Camera.main.backgroundColor = Color.Lerp(from.GetColor(_Vegetation), to.GetColor(_Vegetation), t);
                
                counter += Time.deltaTime;
                
                yield return null;
            }
        }
    }
}
