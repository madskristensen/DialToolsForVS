using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Interop;

using Community.VisualStudio.Toolkit;

using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.PlatformUI.Shell;
using Microsoft.VisualStudio.PlatformUI.Shell.Controls;

using Windows.UI.Input;

namespace DialControllerTools
{
    internal sealed class DialControllerHost : IDialControllerHost
    {
        private readonly Dictionary<IntPtr, RadialController> controllersMapping = new Dictionary<IntPtr, RadialController>();

        private readonly OutputWindowPane outputPane;

        private RadialControllerStatusBarHost status;
        private readonly ImmutableArray<IDialController> controllers;
        private readonly List<IDialController> enabledControllers;
        private IDialController currentController;
        private bool allowRelease;
        private bool wasUsed;

        private IDialController CurrentController
        {
            get => currentController;
            set
            {
                currentController = value;
                status.Text = currentController.Moniker;
            }
        }

        internal DialControllerHost(OutputWindowPane outputPane, in ImmutableArray<IDialController> controllers)
        {
            this.outputPane = outputPane;
            this.controllers = controllers;
            this.enabledControllers = new List<IDialController>(controllers.Length);

            FloatingWindowManager.FloatingWindowCreated += (sender, args) => CreateControllerForWindow(args.Window);
            //var floatingWindowManager = ViewManager.Instance.FloatingWindowManager;
            //floatingWindowManager.

            void OnMainWindowLoaded(object sender, EventArgs args)
            {
                CreateStatusBarItem();
                UpdateEnabledControllers(DialPackage.Options);
            }

            ExecuteWhenWindowIsLoaded(Application.Current.MainWindow, OnMainWindowLoaded);

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainWindow || window is FloatingWindow)
                    CreateControllerForWindow(window);
            }
        }

        private static void ExecuteWhenWindowIsLoaded(Window window, RoutedEventHandler action)
        {
            if (window.IsLoaded) action(window, new RoutedEventArgs());
            else window.Loaded += action;
        }

        private void CreateStatusBarItem()
        {
            status = new RadialControllerStatusBarHost { Name = "PART_DialControllerHost" };
            var injector = new StatusBarInjector(Application.Current.MainWindow);
            injector.InjectControl(status);
        }

        private void CreateControllerForWindow(Window window)
        {
            void CreateControllerForWindowImpl(object sender, EventArgs args)
            {
                window.Loaded -= CreateControllerForWindowImpl;
                IntPtr hwnd = new WindowInteropHelper(window).Handle;
                CreateController(hwnd);
                SetDefaultItems(hwnd);

                if (window.IsActive) ApplyCurrentControllerStateForWindow((Window)sender);
                void OnWindowActivated(object sender, EventArgs args)
                {
                    this.ApplyCurrentControllerStateForWindow((Window)sender);
                }
                window.Activated += OnWindowActivated;

                void RemoveController(object sender, RoutedEventArgs args)
                {
                    window.Activated -= OnWindowActivated;
                    window.Unloaded -= RemoveController;
                    if (controllersMapping.TryGetValue(hwnd, out var controller))
                    {
                        UnsubscribeFromController(controller);
                        controllersMapping.Remove(hwnd);
                    }
                }
                window.Unloaded += RemoveController;
            }

            ExecuteWhenWindowIsLoaded(window, CreateControllerForWindowImpl);
        }

        private void CreateController(IntPtr hwnd)
        {
            var interop = (IRadialControllerInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialController));
            Guid guid = typeof(RadialController).GetInterface("IRadialController").GUID;

            var radialController = interop.CreateForWindow(hwnd, ref guid);
            if (radialController is null)
            {
                outputPane.WriteLine("Couldn't create RadialController");
                return;
            }

            foreach (IDialController controller in enabledControllers)
            {
                radialController.Menu.Items.Add(controller.MenuItem);
            }

            radialController.RotationChanged += OnRotationChanged;
            radialController.ButtonClicked += OnButtonClicked;
            radialController.ControlAcquired += OnControlAcquired;
            //radialController.ControlLost += OnControlLost;
            controllersMapping.Add(hwnd, radialController);
        }

        internal void UnsubscribeFromController(RadialController radialController)
        {
            radialController.RotationChanged -= OnRotationChanged;
            radialController.ButtonClicked -= OnButtonClicked;
            radialController.ControlAcquired -= OnControlAcquired;
            //radialController.ControlLost -= OnControlLost;
        }

        private static void SetDefaultItems(IntPtr hwnd)
        {
            RadialControllerConfiguration config;
            var radialControllerConfigInterop = (IRadialControllerConfigurationInterop)WindowsRuntimeMarshal.GetActivationFactory(typeof(RadialControllerConfiguration));
            Guid guid = typeof(RadialControllerConfiguration).GetInterface("IRadialControllerConfiguration").GUID;

            config = radialControllerConfigInterop.GetForWindow(hwnd, ref guid);
            config.SetDefaultMenuItems(new RadialControllerSystemMenuItemKind[0]);
        }

        private bool UpdateEnabledControllers(Options options)
        {
            var newControllers = controllers.Where(c => options.MenuVisibility[c.Moniker]).ToImmutableArray();
            var anyChanges = !newControllers.SequenceEqual(enabledControllers);
            if (anyChanges)
            {
                enabledControllers.Clear();
                enabledControllers.AddRange(newControllers);
            }

            var defaultMenu = options.DefaultProvider.ToString();
            if (!options.MenuVisibility[defaultMenu])
            {
                defaultMenu = options.MenuVisibility.FirstOrDefault(_ => _.Value).Key ?? nameof(KnownProviders.Scroll);
            }
            CurrentController = enabledControllers.First(c => c.Moniker == defaultMenu);

            return anyChanges;
        }

        internal void OptionsApplied(object sender, EventArgs e)
        {
            var options = (Options)sender;
            var anyControllerChangesMade = UpdateEnabledControllers(options);

            if (!anyControllerChangesMade) return;

            foreach (var controller in controllersMapping.Values)
            {
                var menuItems = controller.Menu.Items;
                for (int i = 0; i < enabledControllers.Count; i++)
                {
                    var newMenuItem = enabledControllers[i].MenuItem;
                    if (i < menuItems.Count)
                    {
                        if (menuItems[i] != enabledControllers[i].MenuItem)
                        {
                            menuItems[i] = newMenuItem;
                        }
                    }
                    else
                    {
                        menuItems.Add(newMenuItem);
                    }
                }
            }
        }

        public void ApplyCurrentControllerStateForWindow(Window window)
        {
            var radialController = controllersMapping[new WindowInteropHelper(window).Handle];
            ApplyCurrentControllerState(radialController);
        }

        private void ApplyCurrentControllerState(RadialController radialController)
        {
            var menuItem = CurrentController.MenuItem;
            if (radialController.Menu.GetSelectedMenuItem() != menuItem)
            {
                radialController.Menu.SelectMenuItem(menuItem);
            }
            radialController.UseAutomaticHapticFeedback = CurrentController.IsHapticFeedbackEnabled;
        }

        public void RequestActivation(IDialController controller)
        {
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (activeWindow is not null)
            {
                allowRelease = true;
                wasUsed = false;
                CurrentController = controller;
                ApplyCurrentControllerStateForWindow(activeWindow);
            }
        }

        public void ReleaseActivation()
        {
            if (wasUsed || !allowRelease)
            {
                wasUsed = false;
                return;
            }

            allowRelease = false;
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (activeWindow is not null)
            {
                var radialController = controllersMapping[new WindowInteropHelper(activeWindow).Handle];
                if (radialController.Menu.TrySelectPreviouslySelectedMenuItem())
                {
                    MenuItemSelected(radialController);
                }
            }
        }

        private void OnControlAcquired(RadialController sender, RadialControllerControlAcquiredEventArgs args)
        {
            //_status.IsActive = true;
            MenuItemSelected(sender);
        }

        private void MenuItemSelected(RadialController radualController)
        {
            RadialControllerMenuItem selectedItem = radualController.Menu.GetSelectedMenuItem();
            if (selectedItem?.DisplayText != CurrentController.MenuItem.DisplayText)
            {
                allowRelease = false;
                // null when window is closed
                var controller = enabledControllers.FirstOrDefault(c => c.MenuItem == selectedItem);
                if (controller is not null)
                {
                    CurrentController = controller;
                    radualController.UseAutomaticHapticFeedback = CurrentController.IsHapticFeedbackEnabled;
                }

            }
        }

        //private void OnControlLost(RadialController sender, object args) => _status.IsActive = false;

        private async void OnButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            if (!await VS.Solutions.IsOpenAsync()) //assume most controllers require a solution
                return;

            wasUsed = true;
            IEnumerable<IDialController> controllers = GetApplicableControllers(sender).Where(c => c.CanHandleClick);
            outputPane.WriteLine("Click: " + string.Join(", ", controllers.Select(c => c.Moniker)));

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
                    outputPane.WriteLine($"Controller {controller.Moniker} failed:");
                    outputPane.WriteLine(ex.ToString());
                }
            }
        }

        private async void OnRotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (!await VS.Solutions.IsOpenAsync()) //assume most controllers require a solution
                return;

            wasUsed = true;
            IEnumerable<IDialController> controllers = GetApplicableControllers(sender).Where(c => c.CanHandleRotate);
            RotationDirection direction = args.RotationDeltaInDegrees > 0 ? RotationDirection.Right : RotationDirection.Left;

            outputPane.WriteLine("Rotate: " + string.Join(", ", controllers.Select(c => c.Moniker)));

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
                    outputPane.WriteLine($"Controller {controller.Moniker} failed:");
                    outputPane.WriteLine(ex.ToString());
                }
            }
        }

        private IEnumerable<IDialController> GetApplicableControllers(RadialController controller)
        {
            string moniker = controller.Menu.GetSelectedMenuItem()?.DisplayText;

            if (string.IsNullOrEmpty(moniker))
                Enumerable.Empty<IDialController>();

            try
            {
                return controllers.Where(c => c.Moniker == moniker);
            }
            catch (Exception ex)
            {
                outputPane.WriteLine($"Cannot retrieve controllers for {moniker}:");
                outputPane.WriteLine(ex.ToString());
                return Enumerable.Empty<IDialController>();
            }
        }
    }
}
