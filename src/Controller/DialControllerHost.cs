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

        [ImportMany(typeof(IDialControllerProvider))]
        private IEnumerable<IDialControllerProvider> _providers { get; set; }

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
        }

        private void SetDefaultItems()
        {
            RadialControllerConfiguration config;
            var radialControllerConfigInterop = (IRadialControllerConfigurationInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialControllerConfiguration));
            Guid guid = typeof(RadialControllerConfiguration).GetInterface("IRadialControllerConfiguration").GUID;

            config = radialControllerConfigInterop.GetForWindow(new IntPtr(VsHelpers.DTE.MainWindow.HWnd), ref guid);
            config.SetDefaultMenuItems(new RadialControllerSystemMenuItemKind[0]);
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
                .Select(provider => provider.TryCreateController(this))
                .Where(controller => controller != null)
                .OrderBy(controller => controller.Specificity).ToArray();

            SetDefaultItems();
        }

        public void AddMenuItem(string moniker, string iconFilePath)
        {
            if (_radialController.Menu.Items.Any(i => i.DisplayText == moniker))
                return;

            IAsyncOperation<StorageFile> operation = StorageFile.GetFileFromPathAsync(iconFilePath);

            operation.Completed += (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    StorageFile file = asyncInfo.GetResults();
                    var stream = RandomAccessStreamReference.CreateFromFile(file);
                    var menuItem = RadialControllerMenuItem.CreateFromIcon(moniker, stream);
                    menuItem.Invoked += MenuItemInvoked;

                    ThreadHelper.Generic.BeginInvoke(DispatcherPriority.Normal, () =>
                    {
                        _radialController.Menu.Items.Add(menuItem);
                    });
                }
            };
        }

        public void AddMenuItem(string moniker, RadialControllerMenuKnownIcon icon)
        {
            if (_radialController.Menu.Items.Any(i => i.DisplayText == moniker))
                return;

            var menuitem = RadialControllerMenuItem.CreateFromKnownIcon(moniker, icon);
            menuitem.Invoked += MenuItemInvoked;
            _radialController.Menu.Items.Add(menuitem);
        }

        private void MenuItemInvoked(RadialControllerMenuItem sender, object args)
        {
            throw new NotImplementedException();
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
                _radialController.Menu.SelectMenuItem(item);
        }

        public void ReleaseActivation()
        {
            _radialController.Menu.TrySelectPreviouslySelectedMenuItem();
        }

        private void OnControlAcquired(RadialController sender, RadialControllerControlAcquiredEventArgs args)
        {
            if (_status != null)
            {
                _status.Activate();
                _status.UpdateSelectedItem(sender.Menu.GetSelectedMenuItem().DisplayText);
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
            var evt = new DialEventArgs();
            IEnumerable<IDialController> controllers = GetApplicableControllers().Where(c => c.CanHandleClick);

            foreach (IDialController controller in controllers)
            {
                controller.OnClick(evt);

                try
                {
                    if (evt.Handled)
                    {
                        if (!string.IsNullOrEmpty(evt.Action))
                        {
                            //VsHelpers.WriteStatus(evt.Action);
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private void OnRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            var evt = new DialEventArgs();
            RotationDirection direction = args.RotationDeltaInDegrees > 0 ? RotationDirection.Right : RotationDirection.Left;
            IEnumerable<IDialController> controllers = GetApplicableControllers().Where(c => c.CanHandleRotate);

            foreach (IDialController controller in controllers)
            {
                try
                {
                    controller.OnRotate(direction, evt);

                    if (evt.Handled)
                    {
                        if (!string.IsNullOrEmpty(evt.Action))
                        {
                            //VsHelpers.WriteStatus(evt.Action);
                        }

                        break;
                    }
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

            var alwaysInclude = new List<string> { PredefinedMonikers.Scroll };

            try
            {
                if (VsHelpers.DTE.ActiveWindow.IsDocument())
                    alwaysInclude.Insert(1, PredefinedMonikers.Editor);

                return _controllers.Where(c => c.Moniker == moniker || alwaysInclude.Contains(c.Moniker));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return Enumerable.Empty<IDialController>();
            }
        }
    }
}
