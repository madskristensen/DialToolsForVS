using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.PlatformUI;

namespace DialToolsForVS
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DialToolsForVS"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:DialToolsForVS;assembly=DialToolsForVS"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:RadialControllerStatusBarItem/>
    ///
    /// </summary>
    [TemplatePart(Name = "PART_TextBlock", Type = typeof(LiveTextBlock))]
    [TemplateVisualState(GroupName = "ActivityStates", Name = RadialControllerStatusBarHost.ActiveStateName)]
    [TemplateVisualState(GroupName = "ActivityStates", Name = RadialControllerStatusBarHost.InactiveStateName)]
    public class RadialControllerStatusBarHost : Control
    {
        internal const string ActiveStateName = "Active";
        internal const string InactiveStateName = "Inactive";

        static RadialControllerStatusBarHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadialControllerStatusBarHost), new FrameworkPropertyMetadata(typeof(RadialControllerStatusBarHost)));
        }

        public RadialControllerStatusBarHost()
        {
            ToolTip = "The Surface Dial is inactive.";
        }

        #region Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(RadialControllerStatusBarHost), new FrameworkPropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                (d as RadialControllerStatusBarHost).IsActive = true;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion Text

        #region IsActive

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(RadialControllerStatusBarHost), new FrameworkPropertyMetadata(false, OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (d as RadialControllerStatusBarHost);
            if ((bool)e.NewValue == true)
            {
                control.ToolTip = "The Surface Dial is active.";
                //bool result = VisualStateManager.GoToState(control, ActiveStateName, false);
            }
            else
            {
                control.ToolTip = "The Surface Dial is inactive.";
                //bool result = VisualStateManager.GoToState(control, InactiveStateName, false);
                control.SetValue(TextProperty, DependencyProperty.UnsetValue);
            }
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        #endregion IsActive
    }
}
