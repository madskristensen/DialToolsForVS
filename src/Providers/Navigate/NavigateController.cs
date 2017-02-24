namespace DialToolsForVS
{
    internal class NavigateController : IDialController
    {
        public string Moniker => NavigateControllerProvider.Moniker;

        public bool CanHandleClick => false;

        public bool CanHandleRotate => true;

        public void OnClick(DialEventArgs e)
        { }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("View.NavigateForward");
            }
            else
            {
                VsHelpers.ExecuteCommand("View.NavigateBackward");
            }

            e.Handled = true;
        }
    }
}
