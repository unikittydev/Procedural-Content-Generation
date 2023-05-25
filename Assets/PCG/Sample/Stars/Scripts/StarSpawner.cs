using PCG.Terrain.Scripts;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;

using Random = Unity.Mathematics.Random;

namespace PCG
{
    public class StarSpawner : MonoBehaviour
    {
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private CustomStarGenerator generator;

        [SerializeField] private uint initialSeed;

        private static readonly ProfilerMarker EMIT_MARKER = new ProfilerMarker("StarSpawner.EmitStars()");
        
        private void OnEnable()
        {
            Random rnd = new Random(initialSeed);
            NativeArray<SampleStar> stars = (NativeArray<SampleStar>)generator.Generate(ref rnd);
            EmitStars(stars);
        }

        private void EmitStars(NativeArray<SampleStar> stars)
        {
            EMIT_MARKER.Begin();
            
            foreach (var star in stars)
                EmitStar(star);
            
            EMIT_MARKER.End();
        }
        
        private void EmitStar(SampleStar star)
        {
            star = new SampleStar()
            {
                mass = star.mass,
                position = star.position,
                luminocity = math.pow(star.mass, 3f),
                radius = math.pow(star.mass, 0.74f),
                temperature = math.pow(star.mass, 0.505f) * 5780f,
            };
            var color = pcgMath.blackbody(star.temperature);
            //var intensity = math.pow(2f, star.luminocity);
            //color *= intensity;
            star.color = new Color(color.x, color.y, color.z, 1f);

            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams()
            {
                position = star.position,
                startColor = star.color,
                startSize = star.radius * 3f,
            };
            ps.Emit(emitParams, 1);
        }
    }
}
