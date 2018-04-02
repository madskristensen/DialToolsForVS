using EnvDTE;

namespace DialToolsForVS
{
    internal class NavigateController : BaseController
    {
        private readonly Commands _commands;

        public override string Moniker => NavigateControllerProvider.Moniker;
        public override bool CanHandleRotate => true;

        public NavigateController( IDialControllerHost host)
        {
            _commands = host.DTE.Commands;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            switch (direction)
            {
                case RotationDirection.Left:
                _commands.ExecuteCommand("View.NavigateBackward");
                    break;
                case RotationDirection.Right:
                _commands.ExecuteCommand("View.NavigateForward");
                    break;
            }

            return true;
        }
    }
}
