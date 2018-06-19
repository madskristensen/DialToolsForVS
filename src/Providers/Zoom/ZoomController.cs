using EnvDTE;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    internal class ZoomController : BaseTextController
    {
        private readonly Commands _commands;

        public override string Moniker => ZoomControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public ZoomController(IDialControllerHost host, IVsTextManager textManager)
            : base(host, textManager)
        {
            _commands = host.DTE.Commands;
        }

        public override bool OnClick()
        {
            IWpfTextView view = GetCurrentTextView();

            if (view == null || view.ZoomLevel == 100)
                return false;

            view.ZoomLevel = 100;
            _commands.ExecuteCommand("View.ZoomOut");
            _commands.ExecuteCommand("View.ZoomIn");

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                return _commands.ExecuteCommand("View.ZoomIn");
            }
            else
            {
                return _commands.ExecuteCommand("View.ZoomOut");
            }
        }
    }
}
