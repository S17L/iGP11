using System.Diagnostics;
using System.IO;

using iGP11.Library;
using iGP11.Tool.Application;
using iGP11.Tool.Domain.Exceptions;
using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.Infrastructure.External
{
    public class InjectionService : IInjectionService
    {
        public InjectionResult Inject(string applicationFilePath, string proxyFilePath)
        {
            var result = StartProcess(applicationFilePath);
            if (result.HasValue && !result.Value)
            {
                throw new DomainOperationException("application not started");
            }

            return new InjectionResult(InjectionDriver.Inject(applicationFilePath, proxyFilePath));
        }

        public bool IsProcessRunning(string applicationFilePath)
        {
            return InjectionDriver.IsProcessRunning(applicationFilePath);
        }

        public bool IsProxyLoaded(string applicationFilePath, string proxyFilePath)
        {
            return InjectionDriver.IsProxyLoaded(applicationFilePath, proxyFilePath);
        }

        private static Process CreateProcess(string applicationFilePath)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = applicationFilePath,
                    WorkingDirectory = Path.GetDirectoryName(applicationFilePath ?? string.Empty)
                }
            };
        }

        private static bool? StartProcess(string applicationFilePath)
        {
            if (applicationFilePath.IsNullOrEmpty() || InjectionDriver.IsProcessRunning(applicationFilePath))
            {
                return null;
            }

            using (var process = CreateProcess(applicationFilePath))
            {
                return process.Start();
            }
        }
    }
}