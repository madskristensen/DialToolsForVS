namespace DialToolsForVS
{
    internal class FindController : BaseController
    {
        public override string Moniker => FindControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override bool OnClick()
        {
            VsHelpers.ExecuteCommand("Edit.FindInFiles");
            return true;
        }
        
        public override bool OnRotate(RotationDirection direction)
        {

            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("Edit.GoToNextLocation");
            }
            else
            {
                VsHelpers.ExecuteCommand("Edit.GoToPrevLocation");
            }

            return true;
        }
    }
}
