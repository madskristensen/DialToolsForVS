using EnvDTE;

namespace DialControllerTools
{
    internal class CustomizableController : BaseController
    {
        private readonly Commands _commands;

        public override string Moniker => CustomizableControllerProvider.Moniker;

        public override bool CanHandleClick => true;

        public override bool CanHandleRotate => true;

        public CustomizableController(IDialControllerHost host)
        {
            _commands = host.DTE.Commands;
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
