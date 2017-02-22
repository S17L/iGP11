using System;
using System.Diagnostics;
using System.IO;

namespace ProductPublisher
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            var logger = new Logger($"{DateTime.Now.Ticks}");
            var updateVersionFactory = new UpdateVersionFactory();

            try
            {
                var configuration = ConfigurationLoader.Load();
                var assemblyInformation = configuration.AssemblyInformation;

                logger.Information("started");
                logger.Information("acquiring last commit id...");

                using (var process = CreateProcess(GetFullPath(configuration.Root, configuration.GetCommitId)))
                {
                    process.Start();
                    process.WaitForExit();

                    var lastCommitId = File.ReadAllText(GetFullPath(configuration.Root, configuration.CommitId)).Trim();
                    assemblyInformation = new AssemblyInformation(assemblyInformation.Version, $"{assemblyInformation.ProductVersion}+{lastCommitId}", assemblyInformation.Copyright);
                    logger.Information($"last commit id: {lastCommitId}");
                }

                logger.Information($"version: {assemblyInformation.Version}, productVersion: {assemblyInformation.ProductVersion}");

                foreach (var relativeFilePath in configuration.Assemblies)
                {
                    var filePath = GetFullPath(configuration.Root, relativeFilePath);

                    updateVersionFactory.Create(filePath)
                        .Update(assemblyInformation);

                    logger.Information($"{filePath} file updated");
                }

                var buildPath = GetFullPath(configuration.Root, configuration.Build);
                logger.Information($"building solution with {buildPath}...");

                using (var process = CreateProcess(buildPath))
                {
                    process.Start();
                    process.WaitForExit();
                    logger.Information("solution built");
                }

                var makePackagePath = GetFullPath(configuration.Root, configuration.MakePackage);
                logger.Information($"making package with {makePackagePath} and copying its content to {assemblyInformation.ProductVersion}");

                using (var process = CreateProcess(makePackagePath, assemblyInformation.ProductVersion))
                {
                    process.Start();
                    process.WaitForExit();
                }

                logger.Information("package made");
            }
            catch (Exception exception)
            {
                logger.Error(exception.ToString());
            }

            logger.Information("stopped");
        }

        private static Process CreateProcess(string filePath, string arguments = null)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = arguments ?? string.Empty,
                    FileName = filePath,
                    WorkingDirectory = Path.GetDirectoryName(filePath ?? string.Empty)
                }
            };
        }

        private static string GetFullPath(string root, string path)
        {
            return Path.GetFullPath(path.Replace("{root}", root));
        }
    }
}