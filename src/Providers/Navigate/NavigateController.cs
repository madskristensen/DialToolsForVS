namespace DialToolsForVS
{
    internal class NavigateController : BaseController
    {
        public override string Moniker => NavigateControllerProvider.Moniker;
        public override bool CanHandleRotate => true;

        public override bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("View.NavigateForward");
            }
            else
            {
                VsHelpers.ExecuteCommand("View.NavigateBackward");
            }

            return true;
        }
    }
}
