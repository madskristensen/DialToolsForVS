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
        private DTE2 _dte;
        private RadialController _radialController;
        private List<IDialController> _controllers;
        private RadialControllerMenuItem _menuItem;
        private StatusBarControl _status;

        [ImportMany(typeof(IDialControllerProvider))]
        private IEnumerable<IDialControllerProvider> _providers { get; set; }

        private DialControllerHost(DTE2 dte)
        {
            _dte = dte;

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

        public static async Tasks.Task InitializeAsync(AsyncPackage package)
        {
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;

            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
            {
                Instance = new DialControllerHost(dte);
            });
        }

        private void CreateController()
        {
            var interop = (IRadialControllerInterop)System.Runtime.InteropServices.WindowsRuntime.WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialController));
            Guid guid = typeof(RadialController).GetInterface("IRadialController").GUID;

            _radialController = interop.CreateForWindow(new IntPtr(_dte.MainWindow.HWnd), ref guid);
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
            foreach (IDialController controller in _controllers.Where(c => c.CanHandleClick))
            {
                if (controller.OnClick(args))
                    break;
            }
        }

        private void OnRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            foreach (IDialController controller in _controllers.Where(c => c.CanHandleRotate))
            {
                RotationDirection direction = args.RotationDeltaInDegrees > 0 ? RotationDirection.Right : RotationDirection.Left;

                if (controller.OnRotate(direction))
                    break;
            }
        }
    }
}
