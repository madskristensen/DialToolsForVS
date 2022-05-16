using EnvDTE;

using EnvDTE80;

using Windows.UI.Input;

namespace DialControllerTools
{
    internal class CustomizableController : BaseController
    {
        private readonly Commands _commands;

        public override string Moniker => CustomizableControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public CustomizableController(RadialControllerMenuItem menuItem, DTE2 dte) : base(menuItem)
        {
            _commands = dte.Commands;
        }

        public override bool OnClick()
        {
            _commands.ExecuteCommand(DialPackage.CustomOptions.ClickAction);
            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            switch (direction)
            {
                case RotationDirection.Left:
                    _commands.ExecuteCommand(DialPackage.CustomOptions.LeftAction);
                    break;
                case RotationDirection.Right:
                    _commands.ExecuteCommand(DialPackage.CustomOptions.RightAction);
                    break;
            }

            return true;
        }
    }
}
