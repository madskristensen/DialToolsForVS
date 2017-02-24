using Microsoft.VisualStudio.Text.Editor;

namespace DialToolsForVS
{
    internal class ZoomController : IDialController
    {
        public string Moniker => PredefinedMonikers.Zoom;
        public Specificity Specificity => Specificity.ToolWindow;

        public bool CanHandleClick => true;

        public bool CanHandleRotate => true;

        public void OnClick(DialEventArgs e)
        {
            if (!VsHelpers.DTE.ActiveWindow.IsDocument())
                return;

            IWpfTextView view = VsHelpers.GetCurentTextView();

            if (view != null && view.HasAggregateFocus)
            {
                view.ZoomLevel = 100;
                e.Handled = true;
            }
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                e.Handled = VsHelpers.ExecuteCommand("View.ZoomIn");
            }
            else
            {
                e.Handled = VsHelpers.ExecuteCommand("View.ZoomOut");
            }
        }
    }
}
