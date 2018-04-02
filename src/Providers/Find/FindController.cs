using EnvDTE;

namespace DialToolsForVS
{
    internal class FindController : BaseController
    {
        private readonly Commands _commands;

        public override string Moniker => FindControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public FindController(IDialControllerHost host)
        {
            _commands = host.DTE.Commands;
        }

        public override bool OnClick()
        {
            _commands.ExecuteCommand("Edit.FindInFiles");
            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            switch (direction)
            {
                case RotationDirection.Left:
                    _commands.ExecuteCommand("Edit.GoToPrevLocation");
                    break;
                case RotationDirection.Right:
                    _commands.ExecuteCommand("Edit.GoToNextLocation");
                    break;
            }

            return true;
        }
    }
}
