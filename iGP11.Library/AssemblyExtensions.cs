using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace iGP11.Library
{
    public static class AssemblyExtensions
    {
        private const string Architecture = "64-bit";

        public static AssemblyInformation GetAssemblyInformation(this Assembly assembly)
        {
            var version = assembly.GetAttribute<AssemblyVersionAttribute>()?.Version;
            var fileVersion = assembly.GetAttribute<AssemblyFileVersionAttribute>()?.Version;
            var informationalVersion = assembly.GetAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            var displayVersion = informationalVersion ?? fileVersion ?? version ?? string.Empty;
            displayVersion = $"v{displayVersion} ({Architecture})";

            return new AssemblyInformation(
                assembly.GetAttribute<AssemblyTitleAttribute>()?.Title,
                assembly.GetAttribute<AssemblyDescriptionAttribute>()?.Description,
                assembly.GetAttribute<AssemblyCompanyAttribute>()?.Company,
                assembly.GetAttribute<AssemblyProductAttribute>()?.Product,
                assembly.GetAttribute<AssemblyCopyrightAttribute>()?.Copyright,
                version,
                fileVersion,
                informationalVersion,
                displayVersion);
        }

        public static IEnumerable<Type> GetImplementations(this Assembly assembly, Type @interface)
        {
            return assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsGenericType && ((@interface.IsGenericType && type.GetInterfaces()
                                                                            .Any(interfaceType => interfaceType.IsGenericType && (interfaceType.GetGenericTypeDefinition() == @interface))) || (!@interface.IsGenericType && type.GetInterfaces()
                                                                                                                                                                                                    .Any(interfaceType => interfaceType == @interface))))
                .ToArray();
        }

        public static IEnumerable<Type> GetImplementations(this IEnumerable<Assembly> assemblies, Type @interface)
        {
            return assemblies.SelectMany(assembly => assembly.GetImplementations(@interface)).ToArray();
        }

        private static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider assembly)
        {
            return (TAttribute)assembly.GetCustomAttributes(typeof(TAttribute), true).SingleOrDefault();
        }
    }
}