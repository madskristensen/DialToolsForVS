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

            if (view.ZoomLevel > 100)
            {
                while (view.ZoomLevel > 100)
                    if (!VsHelpers.ExecuteCommand("View.ZoomOut"))
                        break;
            }
            else
            {
                while (view.ZoomLevel < 100)
                    if (!VsHelpers.ExecuteCommand("View.ZoomIn"))
                        break;
            }

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
