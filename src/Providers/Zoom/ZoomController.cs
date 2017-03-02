using Microsoft.VisualStudio.Text.Editor;

namespace DialToolsForVS
{
    internal class ZoomController : BaseController
    {
        public override string Moniker => ZoomControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override bool OnClick()
        {
            IWpfTextView view = VsHelpers.GetCurentTextView();

            if (view == null || view.ZoomLevel == 100)
                return false;

            view.ZoomLevel = 100;
            VsHelpers.ExecuteCommand("View.ZoomOut");
            VsHelpers.ExecuteCommand("View.ZoomIn");

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                return VsHelpers.ExecuteCommand("View.ZoomIn");
            }
            else
            {
                return VsHelpers.ExecuteCommand("View.ZoomOut");
            }
        }
    }
}
