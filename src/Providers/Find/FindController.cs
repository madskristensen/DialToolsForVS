using EnvDTE;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    internal class FindController : BaseTextController
    {
        private readonly Commands _commands;

        public override string Moniker => FindControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public FindController(IDialControllerHost host, IVsTextManager textManager)
            : base(host, textManager)
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
