using System;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using iGP11.Library;
using iGP11.Library.EventPublisher;
using iGP11.Library.Network;
using iGP11.Tool.Bootstrapper;
using iGP11.Tool.Events;
using iGP11.Tool.Model;
using iGP11.Tool.ViewModel;

namespace iGP11.Tool
{
    public partial class MainWindow : IEventHandler<HideApplicationToTrayEvent>,
                                      IEventHandler<ShowApplicationEvent>,
                                      IEventHandler<ShowErrorEvent>,
                                      IEventHandler<ShutdownEvent>
    {
        private IEventPublisher _eventPublisher;
        private IListener _listener;
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task BootstrapAsync(ApplicationAction action, bool publishApplicationAction)
        {
            try
            {
                await RunAsync(() => Title = $"{Assembly.GetEntryAssembly().GetAssemblyInformation().Product}");
                if (!WindowsPermissionUtility.IsCurrentUserAdministrator())
                {
                    ShowError("application launched without administrator rights");
                    await RunAsync(Shutdown);

                    return;
                }

                await Bootstrapper.Bootstrapper.StartAsync();

                if (publishApplicationAction)
                {
                    await PublishApplicationActionCommandAsync(action);
                    return;
                }

                StartListener();

                _eventPublisher = DependencyResolver.Current.Resolve<IEventPublisher>();
                _eventPublisher.Register<HideApplicationToTrayEvent>(this);
                _eventPublisher.Register<ShowApplicationEvent>(this);
                _eventPublisher.Register<ShowErrorEvent>(this);
                _eventPublisher.Register<ShutdownEvent>(this);

                _viewModel = DependencyResolver.Current.Resolve<MainWindowViewModel>();

                await _viewModel.InjectionConfigurationViewModel.BootstrapAsync(action);
                await RunAsync(() => DataContext = _viewModel);
                await RunAsync(async () => await _viewModel.InjectionConfigurationViewModel.ShowReleaseNotesAsync());
            }
            catch (SecurityException exception)
            {
                await HandleCriticalErrorAsync(exception, "security exception occured");
            }
            catch (UnauthorizedAccessException exception)
            {
                await HandleCriticalErrorAsync(exception, "unauthorized access exception occured");
            }
            catch (Exception exception)
            {
                await HandleCriticalErrorAsync(exception, "unknown exception occured");
            }
        }

        async Task IEventHandler<HideApplicationToTrayEvent>.HandleAsync(HideApplicationToTrayEvent @event)
        {
            await RunAsync(() => WindowState = WindowState.Minimized);
        }

        async Task IEventHandler<ShowApplicationEvent>.HandleAsync(ShowApplicationEvent @event)
        {
            await RunAsync(ShowApplication);
        }

        async Task IEventHandler<ShowErrorEvent>.HandleAsync(ShowErrorEvent @event)
        {
            ShowError(@event.Error);
            await RunAsync(Shutdown);
        }

        async Task IEventHandler<ShutdownEvent>.HandleAsync(ShutdownEvent @event)
        {
            await RunAsync(Shutdown);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _listener?.Dispose();
            _viewModel?.Dispose();

            _eventPublisher?.Unregister<HideApplicationToTrayEvent>(this);
            _eventPublisher?.Unregister<ShowApplicationEvent>(this);
            _eventPublisher?.Unregister<ShowErrorEvent>(this);
            _eventPublisher?.Unregister<ShutdownEvent>(this);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    _eventPublisher?.PublishAsync(new ApplicationMinimizedEvent());
                    break;
                case WindowState.Normal:
                    _eventPublisher?.PublishAsync(new ApplicationRestoredEvent());
                    break;
                case WindowState.Maximized:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.OnStateChanged(e);
        }

        private static void ShowError(string error)
        {
            MessageBox.Show(error, Localization.Localization.Current.Get("FatalErrorOccured"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void Shutdown()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private async Task HandleCriticalErrorAsync(Exception exception, string error)
        {
            Logger.Current.Log(LogLevel.Error, $"{error} while bootstrapping an application; exception: {exception}");
            ShowError(error);
            await RunAsync(Shutdown);
        }

        private async Task PublishApplicationActionCommandAsync(ApplicationAction action)
        {
            try
            {
                Logger.Current.Log(LogLevel.Information, $"publishing command: {action}");
                var output = await DependencyResolver.Current.Resolve<IPublisher>()
                                 .PublishAsync(
                                     new Command(
                                         CommandId.ApplicationActionCommand,
                                         action.ToString()));

                Logger.Current.Log(LogLevel.Information, $"command published: {output}");
            }
            catch (Exception exception)
            {
                Logger.Current.Log(LogLevel.Error, $"unknown exception occured while publishing command: {action}; exception: {exception}");
            }
            finally
            {
                await RunAsync(Shutdown);
            }
        }

        private async Task RunAsync(Action action)
        {
            await Dispatcher.BeginInvoke(action, DispatcherPriority.Send);
        }

        private void ShowApplication()
        {
            Show();
            WindowState = WindowState.Normal;
        }

        private void StartListener()
        {
            try
            {
                _listener = DependencyResolver.Current.Resolve<IListener>();
                _listener.Start();
            }
            catch (Exception exception)
            {
                Logger.Current.Log(LogLevel.Error, $"exception occured while trying to start listener; exception: {exception}");
            }
        }
    }
}