namespace DialToolsForVS
{
    internal class CustomizableController : BaseController
    {
        public override string Moniker => CustomizableControllerProvider.Moniker;

        public override bool CanHandleClick => true;

        public override bool CanHandleRotate => true;

        public override bool OnClick()
        {
            VsHelpers.ExecuteCommand(DialPackage.CustomOptions.ClickAction);
            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand(DialPackage.CustomOptions.RightAction);
            }
            else
            {
                VsHelpers.ExecuteCommand(DialPackage.CustomOptions.LeftAction);
            }
            return true;
        }
    }
}
