using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace DialToolsForVS
{
    internal class StatusBarInjector
    {
        private readonly Panel _panel;

        public StatusBarInjector(Window window)
        {
            _panel = FindChild(window, "StatusBarPanel") as Panel;

            //var wih = new WindowInteropHelper(window);
            //if(TryFindWorkThreadStatusBarContainer(wih.Handle, out FrameworkElement candidate))
            //{
            //    _panel = candidate.Parent as Panel;
            //}
        }

        private bool TryFindWorkThreadStatusBarContainer(IntPtr hwnd, out FrameworkElement candidateElement)
        {
            candidateElement = null;

            HwndSource source = HwndSource.FromHwnd(hwnd);
            var rootVisual = source?.RootVisual as FrameworkElement;
            if (rootVisual == null)
            {
                return false;
            }

            UIElementAutomationPeer statusBarAutomationPeer = this.GetStatusBarAutomationPeer(rootVisual);
            if (statusBarAutomationPeer == null)
            {
                return false;
            }

            candidateElement = statusBarAutomationPeer.Owner as FrameworkElement;
            return candidateElement != null;
        }

        private UIElementAutomationPeer GetStatusBarAutomationPeer(FrameworkElement element)
        {
            UIElementAutomationPeer automationPeer = UIElementAutomationPeer.CreatePeerForElement(element) as UIElementAutomationPeer;

            return this.EnumerateElement(automationPeer, peer =>
                peer?.GetAutomationControlType() == AutomationControlType.StatusBar
             && peer.GetAutomationId() == "StatusBarContainer");
        }

        private UIElementAutomationPeer EnumerateElement(UIElementAutomationPeer peer, Predicate<UIElementAutomationPeer> predicate)
        {
            foreach (UIElementAutomationPeer automationPeer in peer.GetChildren())
                if (predicate(automationPeer))
                {
                    return automationPeer;
                }

            foreach (UIElementAutomationPeer automationPeer in peer.GetChildren())
            {
                peer = EnumerateElement(automationPeer, predicate);
                if (peer != null)
                    return peer;
            }

            return null;
        }


        private static DependencyObject FindChild(DependencyObject parent, string childName)
        {
            if (parent == null)
            {
                return null;
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                {
                    return frameworkElement;
                }
            }

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                child = StatusBarInjector.FindChild(child, childName);

                if (child != null)
                {
                    return child;
                }
            }

            return null;
        }

        public void InjectControl(FrameworkElement control)
         => _panel.Children.Insert(1, control);

        public JoinableTask<bool> IsInjectedAsync(FrameworkElement control)
         => ThreadHelper.JoinableTaskFactory.RunAsync(VsTaskRunContext.UIThreadNormalPriority,
        () => Task.FromResult(_panel.Children.Contains(control)));

        public JoinableTask UninjectControlAsync(FrameworkElement control) => ThreadHelper.JoinableTaskFactory
            .StartOnIdle(() =>
            _panel.Children.Remove(control), VsTaskRunContext.UIThreadNormalPriority);
    }
}