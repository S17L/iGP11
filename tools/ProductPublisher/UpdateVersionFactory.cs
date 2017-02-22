using System;

namespace ProductPublisher
{
    internal class UpdateVersionFactory
    {
        public IUpdateAssemblyInformationPolicy Create(string filePath)
        {
            if (filePath.EndsWith(".cs"))
            {
                return new ManagedAssemblyInformationUpdatePolicy(filePath);
            }

            if (filePath.EndsWith(".rc"))
            {
                return new UnmanagedAssemblyInformationUpdatePolicy(filePath);
            }

            throw new NotSupportedException($"file path: {filePath} could not be resolved");
        }
    }
}