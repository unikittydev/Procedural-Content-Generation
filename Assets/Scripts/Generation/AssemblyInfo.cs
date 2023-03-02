using PCG.Generation;
using Unity.Jobs;

[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<UniformIntGenerator, int>))]
[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<UniformFloatGenerator, float>))]
[assembly: RegisterGenericJobType(typeof(GenerateFieldJob<UniformDoubleGenerator, double>))]
