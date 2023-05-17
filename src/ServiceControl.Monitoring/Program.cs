namespace ServiceControl.Monitoring
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using NServiceBus.Logging;

    static class Program
    {
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) => ResolveAssembly(e.Name);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => LogException(e.ExceptionObject as Exception);

            var arguments = new HostArguments(args);

            var settings = LoadSettings(arguments);

            var runAsWindowsService = !Environment.UserInteractive && !arguments.Portable;
            LoggingConfigurator.Configure(settings, !runAsWindowsService);

            await new CommandRunner(arguments.Commands)
                .Run(settings)
                .ConfigureAwait(false);
        }

        static Settings LoadSettings(HostArguments args)
        {
            var settings = new Settings();
            args.ApplyOverridesTo(settings);
            return settings;
        }

        static void LogException(Exception ex)
        {
            Logger.Error("Unhandled exception was caught.", ex);
        }

        static Assembly ResolveAssembly(string name)
        {
            var assemblyLocation = Assembly.GetEntryAssembly().Location;
            var appDirectory = Path.GetDirectoryName(assemblyLocation);
            var requestingName = new AssemblyName(name).Name;

            var combine = Path.Combine(appDirectory, requestingName + ".dll");
            var assembly = !File.Exists(combine) ? null : Assembly.LoadFrom(combine);
            if (assembly == null)
            {
                //look into transport directory
                var transportsPath = Path.Combine(appDirectory, "Transports");
                var file = Directory.EnumerateFiles(transportsPath, requestingName + ".dll", SearchOption.AllDirectories).SingleOrDefault();
                if (file != null)
                {
                    assembly = Assembly.LoadFrom(file);
                }
            }

            return assembly;
        }

        static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
    }
}