using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DialToolsForVS
{
    internal class StatusBarControl
    {
        private static Ellipse _ellipse;
        private static TextBlock _text;

        public StatusBarControl()
        {
            Control = CreateControl();
        }

        public FrameworkElement Control { get; }

        public static FrameworkElement CreateControl()
        {
            _ellipse = new Ellipse
            {
                Width = 12,
                Height = 12,
                Margin = new Thickness(5, 1, 0, 0),
                Stroke = Brushes.WhiteSmoke,
                StrokeThickness = 2
            };

            _text = new TextBlock
            {
                Text = string.Empty,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Medium,
                Margin = new Thickness(5, 4, 7, 0),
                Visibility = Visibility.Collapsed,
            };

            var panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };

            panel.Children.Add(_ellipse);
            panel.Children.Add(_text);

            return panel;
        }

        public void UpdateSelectedItem(string displayText)
        {
            _text.Text = displayText;
            _text.Visibility = Visibility.Visible;
        }

        public void Activate(string displayText)
        {
            _ellipse.Fill = Brushes.White;
            Control.ToolTip = "The Surface Dial is active";
            UpdateSelectedItem(displayText);
        }

        public void Deactivate()
        {
            _ellipse.Fill = Brushes.Transparent;
            Control.ToolTip = "The Surface Dial is inactive. Select the Visual Studio item from the Surface Dial menu to activate.";
            _text.Visibility = Visibility.Collapsed;
        }
    }
}
