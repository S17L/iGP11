using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using iGP11.Library;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Model;

namespace iGP11.Tool
{
    public partial class App
    {
        private static readonly Guid _id = new Guid("{69894708-BB5A-4A3F-82B3-D4199B7C16C7}");

        private Mutex _mutex;

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.Dispose();

            Logger.Current.Log("application stopped");
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            bool isCreated;
            _mutex = new Mutex(true, _id.ToString(), out isCreated);

            await Bootstrapper.Bootstrapper.PreStart();
            Dispatcher.UnhandledException += DispatcherOnUnhandledException;

            var mainWindow = new MainWindow();
            if (isCreated)
            {
                mainWindow.Show();
            }

            await Task.Run(() => mainWindow.BootstrapAsync(GetApplicationAction(e.Args), !isCreated));
        }

        private static void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            Logger.Current.Log(LogLevel.Error, $"unhandled application error occured; exception: {dispatcherUnhandledExceptionEventArgs.Exception}");
            dispatcherUnhandledExceptionEventArgs.Handled = true;
        }

        private static ApplicationAction GetApplicationAction(IList<string> arguments)
        {
            if (arguments.IsNullOrEmpty())
            {
                return ApplicationAction.Default;
            }

            ApplicationAction action;
            return Enum.TryParse(arguments[0].Trim(), true, out action)
                       ? action
                       : ApplicationAction.Default;
        }
    }
}