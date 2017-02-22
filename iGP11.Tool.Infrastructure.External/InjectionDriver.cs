using System.Runtime.InteropServices;

namespace iGP11.Tool.Infrastructure.External
{
    internal static class InjectionDriver
    {
        private const CallingConvention Convention = CallingConvention.Cdecl;
        private const string InjectionLibrary = "iGP11.External.Injection.dll";

        [DllImport(InjectionLibrary, CallingConvention = Convention, EntryPoint = "inject")]
        internal static extern ulong Inject(string applicationFilePath, string proxyFilePath);

        [DllImport(InjectionLibrary, CallingConvention = Convention, EntryPoint = "isProcessRunning")]
        internal static extern bool IsProcessRunning(string applicationFilePath);

        [DllImport(InjectionLibrary, CallingConvention = Convention, EntryPoint = "isProxyLoaded")]
        internal static extern bool IsProxyLoaded(string applicationFilePath, string proxyFilePath);
    }
}