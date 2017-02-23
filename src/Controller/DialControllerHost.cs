using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Input;
using Tasks = System.Threading.Tasks;

namespace DialToolsForVS
{
    internal class DialControllerHost : IDialControllerHost
    {
        private RadialController _radialController;
        private List<IDialController> _controllers;
        private RadialControllerMenuItem _menuItem;
        private StatusBarControl _status;

        [ImportMany(typeof(IDialControllerProvider))]
        private IEnumerable<IDialControllerProvider> _providers { get; set; }

        private DialControllerHost()
        {
            CreateController();
            CreateStatusBar();
            HookUpEvents();
            AddMenuItem();
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

        private void CreateController()
        {
            var interop = (IRadialControllerInterop)System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialController));
            Guid guid = typeof(RadialController).GetInterface("IRadialController").GUID;

            _radialController = interop.CreateForWindow(new IntPtr(VsHelpers.DTE.MainWindow.HWnd), ref guid);
        }

        private void AddMenuItem()
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = Path.Combine(folder, "Resources\\DialIcon.png");
            IAsyncOperation<StorageFile> getItemImageOperation = StorageFile.GetFileFromPathAsync(filePath);

            getItemImageOperation.Completed += new AsyncOperationCompletedHandler<StorageFile>(AddMenuItemFromImage);
        }

        private void AddMenuItemFromImage(IAsyncOperation<StorageFile> asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Completed)
            {
                StorageFile imageFile = asyncInfo.GetResults();
                _menuItem = RadialControllerMenuItem.CreateFromIcon("Visual Studio", RandomAccessStreamReference.CreateFromFile(imageFile));
                _radialController.Menu.Items.Add(_menuItem);

                RequestActivation();
            }
        }

        private void CreateStatusBar()
        {
            _status = new StatusBarControl();
            var injector = new StatusBarInjector(Application.Current.MainWindow);
            injector.InjectControl(_status.Control);
        }

        public void RequestActivation()
        {
            if (!_radialController.Menu.Items.Contains(_menuItem) || _radialController.Menu.GetSelectedMenuItem() == _menuItem)
                return;

            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
            {
                _radialController.Menu.SelectMenuItem(_menuItem);
            });
        }

        private void HookUpEvents()
        {
            _radialController.RotationChanged += OnRotationChanged;
            _radialController.ButtonClicked += OnButtonClicked;
            _radialController.ControlAcquired += OnControlAcquired;
            _radialController.ControlLost += OnControlLost;
        }

        private void OnControlAcquired(RadialController sender, RadialControllerControlAcquiredEventArgs args)
        {
            if (_providers == null)
            {
                this.SatisfyImportsOnce();
                _controllers = new List<IDialController>();

                foreach (IDialControllerProvider provider in _providers)
                {
                    IDialController controller = provider.TryCreateController(this);

                    if (controller != null)
                        _controllers.Add(controller);
                }

                _controllers.Sort((x, y) => x.Specificity.CompareTo(y.Specificity));
            }

            _status.Activate();
            VsHelpers.WriteStatus("Dial activated");
        }

        private void OnControlLost(RadialController sender, object args)
        {
            if (_status != null)
            {
                _status.Deactivate();
            }

            VsHelpers.WriteStatus("Dial deactivated");
        }

        private void OnButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            var evt = new DialEventArgs();

            foreach (IDialController controller in _controllers.Where(c => c.CanHandleClick))
            {
                controller.OnClick(args, evt);

                try
                {
                    if (evt.Handled)
                    {
                        if (!string.IsNullOrEmpty(evt.Action))
                        {
                            VsHelpers.WriteStatus(evt.Action);
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                    // TODO: implement logger
                }
            }
        }

        private void OnRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            var evt = new DialEventArgs();
            RotationDirection direction = args.RotationDeltaInDegrees > 0 ? RotationDirection.Right : RotationDirection.Left;

            foreach (IDialController controller in _controllers.Where(c => c.CanHandleRotate))
            {
                try
                {
                    controller.OnRotate(direction, evt);

                    if (evt.Handled)
                    {
                        if (!string.IsNullOrEmpty(evt.Action))
                        {
                            VsHelpers.WriteStatus(evt.Action);
                        }

                        break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                    // TODO: implement logger
                }
            }
        }
    }
}
