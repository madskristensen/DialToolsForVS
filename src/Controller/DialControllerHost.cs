using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Task = System.Threading.Tasks.Task;

namespace DialToolsForVS
{
    internal class DialControllerHost : IDialControllerHost
    {
        private static DTE2 dte;
        private RadialControllerStatusBarHost _status;
        private RadialController _radialController;
        private ImmutableArray<IDialController> _controllers;
        private bool _firstActivation = true;

        [ImportMany(typeof(IDialControllerProvider))]
        private IEnumerable<Lazy<IDialControllerProvider, IDialMetadata>> _providers { get; set; }

        private DialControllerHost() { }

        public static DialControllerHost Instance
        {
            get;
            private set;
        }

        public DTE2 DTE => dte;

        public static async Task InitializeAsync(CancellationToken cancellationToken)
        {
            void InitializeRadialController()
            {
                Instance.CreateController();
                Instance.HookUpEvents();
            }

            dte = await VsHelpers.GetDteAsync(cancellationToken);
            Instance = new DialControllerHost();
            await Task.WhenAll(
                Instance.CreateStatusBarItemAsync().JoinAsync(),
                ThreadHelper.JoinableTaskFactory.StartOnIdle(new Action(InitializeRadialController)).JoinAsync());
            await Instance.SatisfyImportsOnceAsync(cancellationToken);
            await Instance.ImportProvidersAsync(cancellationToken);
        }

        private JoinableTask CreateStatusBarItemAsync()
        {
            _status = new RadialControllerStatusBarHost { Name = "PART_DialControllerHost" };
            var injector = new StatusBarInjector(Application.Current.MainWindow);
            return injector.InjectControlAsync(_status);
        }

        private void CreateController()
        {
            var interop = (IRadialControllerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialController));
            Guid guid = typeof(RadialController).GetInterface("IRadialController").GUID;

            _radialController = interop.CreateForWindow(new IntPtr(dte.MainWindow.HWnd), ref guid);

            if (_radialController == null)
                Logger.Log("Couldn't create RadialController");
        }

        private JoinableTask SetDefaultItemsAsync() => ThreadHelper.JoinableTaskFactory.StartOnIdle(() =>
        {
            RadialControllerConfiguration config;
            var radialControllerConfigInterop = (IRadialControllerConfigurationInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialControllerConfiguration));
            Guid guid = typeof(RadialControllerConfiguration).GetInterface("IRadialControllerConfiguration").GUID;

            config = radialControllerConfigInterop.GetForWindow(new IntPtr(dte.MainWindow.HWnd), ref guid);
            config.SetDefaultMenuItems(new RadialControllerSystemMenuItemKind[0]);
        });

        private void HookUpEvents()
        {
            Debug.Assert(_radialController != null);
            _radialController.RotationChanged += OnRotationChanged;
            _radialController.ButtonClicked += OnButtonClicked;
            _radialController.ControlAcquired += OnControlAcquired;
            _radialController.ControlLost += OnControlLost;
            DialPackage.Options.OptionsApplied += OptionsApplied;
        }

        private void OptionsApplied(object sender, EventArgs e)
        {
            _radialController.Menu.Items.ToList().ForEach(_ => RemoveMenuItem(_.DisplayText));
            ThreadHelper.JoinableTaskFactory.Run(() => ImportProvidersAsync());
        }

        private async Task ImportProvidersAsync(CancellationToken cancellationToken = default)
        {
            var tasks = _providers
                .Select(async provider =>
                {
                    var controller = await provider.Value.TryCreateControllerAsync(this, cancellationToken);
                    return (Controller: controller, provider.Metadata.Order);
                });
            _controllers = (await Task.WhenAll(tasks))
                           .Where(result => result.Controller != null)
                           .OrderBy(result => result.Order)
                           .Select(result => result.Controller)
                           .ToImmutableArray();

            await SetDefaultItemsAsync();
        }

        public async Task AddMenuItemAsync(string moniker, string iconFilePath)
        {
            if (_radialController.Menu.Items.Any(i => i.DisplayText == moniker))
                return;
            if (!DialPackage.Options.MenuVisibility[moniker])
                return;

            await Task.Yield();
            await TaskScheduler.Default;

            StorageFile file = await StorageFile.GetFileFromPathAsync(iconFilePath);

            var stream = RandomAccessStreamReference.CreateFromFile(file);
            var menuItem = RadialControllerMenuItem.CreateFromIcon(moniker, stream);

            menuItem.Invoked += (sender, args) =>
            {
                _status.Text = sender.DisplayText;
                _controllers.FirstOrDefault(c => c.Moniker == moniker)?.OnActivate();
            };

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            _radialController.Menu.Items.Add(menuItem);
        }

        public void RemoveMenuItem(string moniker)
        {
            RadialControllerMenuItem item = _radialController.Menu.Items.FirstOrDefault(i => i.DisplayText == moniker);

            if (item != null && _radialController.Menu.Items.Contains(item))
                _radialController.Menu.Items.Remove(item);
        }

        public void RequestActivation(string moniker)
        {
            RadialControllerMenuItem item = _radialController.Menu.Items.FirstOrDefault(i => i.DisplayText == moniker);

            if (item != null)
            {
                _radialController.Menu.SelectMenuItem(item);
                _status.Text = item.DisplayText;
            }
        }

        public void ReleaseActivation()
        {
            _radialController.Menu.TrySelectPreviouslySelectedMenuItem();
        }

        private void OnControlAcquired(RadialController sender, RadialControllerControlAcquiredEventArgs args)
        {
            if (_firstActivation)
            {
                _firstActivation = false;
                var defaultMenu = DialPackage.Options.DefaultProvider.ToString();
                if (!DialPackage.Options.MenuVisibility[defaultMenu])
                {
                    defaultMenu = DialPackage.Options.MenuVisibility.FirstOrDefault(_ => _.Value).Key ?? KnownProviders.Scroll.ToString();
                }
                RequestActivation(defaultMenu);
            }

            Debug.Assert(_status != null);
            _status.Text = sender.Menu.GetSelectedMenuItem()?.DisplayText;
        }

        private void OnControlLost(RadialController sender, object args)
        {
            Debug.Assert(_status != null);
            _status.IsActive = false;
        }

        private void OnButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            IEnumerable<IDialController> controllers = GetApplicableControllers().Where(c => c.CanHandleClick);
            Logger.Log("Click: " + string.Join(", ", controllers.Select(c => c.Moniker)));

            foreach (IDialController controller in controllers)
            {
                try
                {
                    bool handled = controller.OnClick();

                    if (handled)
                        break;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private void OnRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            IEnumerable<IDialController> controllers = GetApplicableControllers().Where(c => c.CanHandleRotate);
            RotationDirection direction = args.RotationDeltaInDegrees > 0 ? RotationDirection.Right : RotationDirection.Left;

            Logger.Log("Rotate: " + string.Join(", ", controllers.Select(c => c.Moniker)));

            foreach (IDialController controller in controllers)
            {
                try
                {
                    bool handled = controller.OnRotate(direction);

                    if (handled)
                        break;
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private IEnumerable<IDialController> GetApplicableControllers()
        {
            string moniker = _radialController?.Menu.GetSelectedMenuItem()?.DisplayText;

            if (string.IsNullOrEmpty(moniker))
                Enumerable.Empty<IDialController>();

            try
            {
                return _controllers.Where(c => c.Moniker == moniker);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return Enumerable.Empty<IDialController>();
            }
        }

    }
}
