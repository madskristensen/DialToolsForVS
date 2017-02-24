namespace DialToolsForVS
{
    internal class NavigateController : IDialController
    {
        public string Moniker => NavigateControllerProvider.Moniker;
        public bool CanHandleClick => false;
        public bool CanHandleRotate => true;
        public bool OnClick() => false;

        public bool OnRotate(RotationDirection direction)
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
