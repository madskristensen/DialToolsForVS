using EnvDTE;
using EnvDTE80;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class DebugController : IDialController
    {
        private DTE2 _dte = VsHelpers.DTE;
        private DebuggerEvents _events;

        public DebugController(IDialControllerHost host)
        {
            _events = _dte.Events.DebuggerEvents;
            _events.OnEnterBreakMode += delegate { host.RequestActivation(); };
        }


        public Specificity Specificity => Specificity.IdeState;

        public bool CanHandleClick
        {
            get { return _dte.Application?.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode; }
        }

        public bool CanHandleRotate
        {
            get { return CanHandleClick; }
        }

        public void OnClick(RadialControllerButtonClickedEventArgs args, DialEventArgs e)
        {
            _dte.Application.Debugger.StepInto();

            e.Action = "Step into";
            e.Handled = true;
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                _dte.Application.Debugger.StepOver();
                e.Action = "Step over";
            }
            else
            {
                _dte.Application.Debugger.StepOut();
                e.Action = "Step out";
            }

            e.Handled = true;
        }
    }
}
