using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
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
        }

        {
            {
            }

            {
            }

        }

        {
        }

        {

            {



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

        public JoinableTask InjectControlAsync(FrameworkElement pControl) => ThreadHelper.JoinableTaskFactory
            .StartOnIdle(() =>
            _panel.Children.Insert(1, pControl), VsTaskRunContext.UIThreadNormalPriority);

        public JoinableTask<bool> IsInjectedAsync(FrameworkElement pControl)
         => ThreadHelper.JoinableTaskFactory.RunAsync(VsTaskRunContext.UIThreadNormalPriority,
        () => Task.FromResult(_panel.Children.Contains(pControl)));

        public JoinableTask UninjectControlAsync(FrameworkElement pControl) => ThreadHelper.JoinableTaskFactory
            .StartOnIdle(() =>
            _panel.Children.Remove(pControl), VsTaskRunContext.UIThreadNormalPriority);
    }
}