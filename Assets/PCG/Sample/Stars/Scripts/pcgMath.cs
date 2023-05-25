using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace PCG
{
    public static class pcgMath
    {
        public static float3 blackbody(float Temperature)
        {
            float3 color = float3(255.0f, 255.0f, 255.0f);
            color.x = 56100000f * pow(Temperature, -1.5f) + 148f;
            color.y = 100.04f * log(Temperature) - 623.6f;
            
            if (Temperature > 6500f)
                color.y = 35200000f * pow(Temperature, -1.5f) + 184f;
            
            color.z = 194.18f * log(Temperature) - 1448.6f;
            color = clamp(color, 0f, 255f) / 255f;
            
            if (Temperature < 1000f)
                color *= Temperature * .001f;
            
            return color;
        }
    }
}
