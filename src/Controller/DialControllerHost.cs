using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class DialControllerHost : IDialControllerHost
    {
        private RadialController _radialController;
        private IEnumerable<IDialController> _controllers;
        private StatusBarControl _status;
        private bool _firstActivation = true;

        [ImportMany(typeof(IDialControllerProvider))]
        private IEnumerable<Lazy<IDialControllerProvider, IDialMetadata>> _providers { get; set; }

        private DialControllerHost()
        {
            CreateStatusBar();
            CreateController();
            HookUpEvents();
            ImportProviders();
        }

        public static DialControllerHost Instance
        {
            get;
            private set;
        }

        public static void Initialize()
        {
            Instance = new DialControllerHost();
        }

        private void CreateStatusBar()
        {
            _status = new StatusBarControl();
            var injector = new StatusBarInjector(Application.Current.MainWindow);
            injector.InjectControl(_status.Control);
        }

        private void CreateController()
        {
            var interop = (IRadialControllerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialController));
            Guid guid = typeof(RadialController).GetInterface("IRadialController").GUID;

            _radialController = interop.CreateForWindow(new IntPtr(VsHelpers.DTE.MainWindow.HWnd), ref guid);

            if (_radialController == null)
                Logger.Log("Coulnd't create RadialController");
        }

        private void SetDefaultItems()
        {
            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
            {
                RadialControllerConfiguration config;
                var radialControllerConfigInterop = (IRadialControllerConfigurationInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialControllerConfiguration));
                Guid guid = typeof(RadialControllerConfiguration).GetInterface("IRadialControllerConfiguration").GUID;

                config = radialControllerConfigInterop.GetForWindow(new IntPtr(VsHelpers.DTE.MainWindow.HWnd), ref guid);
                config.SetDefaultMenuItems(new RadialControllerSystemMenuItemKind[0]);
            });
        }

        private void HookUpEvents()
        {
            if (_radialController != null)
            {
                _radialController.RotationChanged += OnRotationChanged;
                _radialController.ButtonClicked += OnButtonClicked;
                _radialController.ControlAcquired += OnControlAcquired;
                _radialController.ControlLost += OnControlLost;
            }
        }

        private void ImportProviders()
        {
            this.SatisfyImportsOnce();

            _controllers = _providers
                .OrderBy(provider => provider.Metadata.Order)
                .Select(provider => provider.Value.TryCreateController(this))
                .Where(controller => controller != null)
                .ToArray();

            SetDefaultItems();
        }

        public void AddMenuItem(string moniker, string iconFilePath)
        {
            if (_radialController.Menu.Items.Any(i => i.DisplayText == moniker))
                return;
            if (!DialPackage.Options.MenuVisibility[moniker])
                return;

            IAsyncOperation <StorageFile> operation = StorageFile.GetFileFromPathAsync(iconFilePath);

            operation.Completed += (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    StorageFile file = asyncInfo.GetResults();
                    var stream = RandomAccessStreamReference.CreateFromFile(file);
                    var menuItem = RadialControllerMenuItem.CreateFromIcon(moniker, stream);

                    menuItem.Invoked += (sender, args) =>
                    {
                        _status.UpdateSelectedItem(sender.DisplayText);
                        _controllers.FirstOrDefault(c => c.Moniker == moniker)?.OnActivate();
                    };

                    ThreadHelper.Generic.BeginInvoke(DispatcherPriority.Normal, () =>
                    {
                        _radialController.Menu.Items.Add(menuItem);
                    });
                }
            };
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
                _status.UpdateSelectedItem(item.DisplayText);
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

            if (_status != null)
            {
                _status.Activate(sender.Menu.GetSelectedMenuItem()?.DisplayText);
            }
        }

        private void OnControlLost(RadialController sender, object args)
        {
            if (_status != null)
            {
                _status.Deactivate();
            }
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
