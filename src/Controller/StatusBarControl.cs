using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DialToolsForVS
{
    internal class StatusBarControl
    {
        public StatusBarControl()
        {
            Control = CreateControl();
        }

        public Ellipse Control { get; }

        public static Ellipse CreateControl()
        {
            return new Ellipse
            {
                Width = 12,
                Height = 12,
                Margin = new Thickness(5, 3, 0, 5),
                Stroke = Brushes.WhiteSmoke,
                StrokeThickness = 2
            };
        }

        public void Activate()
        {
            Control.Fill = Brushes.White;
            Control.ToolTip = "The Surface Dial is active";
        }

        public void Deactivate()
        {
            Control.Fill = Brushes.Transparent;
            Control.ToolTip = "The Surface Dial is inactive. Select the Visual Studio item from the Surface Dial menu to activate.";
        }
    }
}
