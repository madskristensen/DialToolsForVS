using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class ZoomController : IDialController
    {
        public string Moniker => PredefinedMonikers.Zoom;
        public Specificity Specificity => Specificity.ToolWindow;

        public bool CanHandleClick => false;

        public bool CanHandleRotate => true;

        public void OnClick(RadialControllerButtonClickedEventArgs args, DialEventArgs e)
        {
            VsHelpers.ExecuteCommand("View.Zoom", 100);
            e.Handled = true;
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("View.ZoomIn");
            }
            else
            {
                VsHelpers.ExecuteCommand("View.ZoomOut");
            }

            e.Handled = true;
        }
    }
}
