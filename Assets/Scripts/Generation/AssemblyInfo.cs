using PCG.Generation;
using Unity.Jobs;
using Unity.Mathematics;

[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<UniformIntGenerator, int>))]
[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<UniformFloatGenerator, float>))]
[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<UniformFloat3Generator, float3>))]
[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<UniformDoubleGenerator, double>))]
