using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    internal class NavigateController : BaseTextController
    {
        private readonly Commands _commands;

        public override string Moniker => NavigateControllerProvider.Moniker;
        public override bool CanHandleRotate => true;

        public NavigateController(IDialControllerHost host, IVsTextManager textManager)
            : base(host, textManager)
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
