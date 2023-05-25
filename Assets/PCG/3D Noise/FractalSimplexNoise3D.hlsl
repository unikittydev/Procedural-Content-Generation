#include "NoiseShader/HLSL/SimplexNoise3D.hlsl"
#include "NoiseShader/HLSL/SimplexNoise2D.hlsl"
#include "NoiseShader/HLSL/Voronoi3D.hlsl"

const float maxFloat = 3.402823466e+38F;

float noise3D(float3 input, float scale, float power)
{
	float value;
	SimplexNoise3D_float(input * scale, value);
	return (1.0f + value) * .5f * power;
}

float noise2D(float2 input, float scale, float power)
{
	float value;
	SimplexNoise2D_float(input * scale, value);
	return (1.0f + value) * .5f * power;
}

void FractalSimplexNoise3D_float(float3 input, float persistence, float scale, float power, float octaves, out float value)
{
	float height = 0, totalPower = 0;
	for (int i = 0; i < octaves; i++)
	{
		height += noise3D(input, scale, power);
		totalPower += power;
		scale *= 2;
		power *= persistence;
	}
	value = height / totalPower;
}

void FractalSimplexNoise2D_float(float2 input, float persistence, float scale, float power, float octaves, out float value)
{
	float height = 0, totalPower = 0;
	for (int i = 0; i < octaves; i++)
	{
		height += noise2D(input, scale, power);
		totalPower += power;
		scale *= 2;
		power *= persistence;
	}
	value = height / totalPower;
}

void FractalVoronoi3D_float(float3 input, float persistence, float angleOffset, float cellDensity, float power, float octaves, out float value, out float cells)
{
	float totalValue = 0, totalCells = 0;
	float totalPower = 0;
	float _value, _cells;

	for (int i = 0; i < octaves; i++)
	{
		Voronoi3D_float(input, angleOffset, cellDensity, _value, _cells);
		totalPower += power;
		totalValue += _value * power;
		totalCells += _cells * power;
		cellDensity *= 2;
		power *= persistence;
	}
	value = totalValue / totalPower;
	cells = totalCells / totalPower;
}

void FractalWarpedSimplexNoise3D_float(float3 input, float persistence, float scale, float power, float warpOffset, float warpScale, float octaves, out float value)
{
	float x, y, z;
	FractalSimplexNoise3D_float(input + float3(warpOffset, 0, 0), persistence, scale, power, octaves, x);
	FractalSimplexNoise3D_float(input + float3(0, warpOffset, 0), persistence, scale, power, octaves, y);
	FractalSimplexNoise3D_float(input + float3(0, 0, warpOffset), persistence, scale, power, octaves, z);
	FractalSimplexNoise3D_float(input + warpScale * float3(x, y, z), persistence, scale, power, octaves, value);
}

float2 raySphere(float3 center, float radius, float3 origin, float3 direction)
{
	float3 offset = origin - center;
	float a = 1;
	float b = 2.0 * dot(offset, direction);
	float c = dot(offset, offset) - radius * radius;
	float discriminant = b * b - 4 * a * c;

	if (discriminant > 0)
	{
		float s = sqrt(discriminant);
		float distToSphereNear = max(0, (-b - s) / (2 * a));
		float distToSphereFar = (-b + s) / (2 * a);

		if (distToSphereFar >= 0)
		{
			return float2(distToSphereNear, distToSphereFar - distToSphereNear);
		}
	}
	return float2(maxFloat, 0);
}

void HitSphere_float(float3 center, float radius, float3 origin, float3 direction, out float distToSphere, out float distThroughSphere)
{
	float2 ray = raySphere(center, radius, origin, direction);
	distToSphere = ray.x;
	distThroughSphere = ray.y;
}