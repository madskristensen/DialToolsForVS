using EnvDTE;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class DebugController : IDialController
    {
        private DebuggerEvents _events;

        public DebugController(IDialControllerHost host)
        {
            _events = VsHelpers.DTE.Events.DebuggerEvents;
            _events.OnEnterBreakMode += delegate { host.RequestActivation(Moniker); };
            _events.OnEnterDesignMode += delegate { host.ReleaseActivation(); };
        }

        public string Moniker => DebugControllerProvider.Moniker;
        public Specificity Specificity => Specificity.IdeState;
        public bool CanHandleClick => true;
        public bool CanHandleRotate
        {
            get { return VsHelpers.DTE.Application?.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode; }
        }

        public void OnClick(RadialControllerButtonClickedEventArgs args, DialEventArgs e)
        {
            dbgDebugMode? debugMode = VsHelpers.DTE.Application?.Debugger.CurrentMode;

            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                VsHelpers.DTE.Application.Debugger.StepInto();
                e.Action = "Step into";
            }
            else if (debugMode == dbgDebugMode.dbgDesignMode)
            {
                VsHelpers.ExecuteCommand("Debug.ToggleBreakpoint");
                e.Action = "Toggle breakpoint";
            }

            e.Handled = true;
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.DTE.Application.Debugger.StepOver();
                e.Action = "Step over";
            }
            else
            {
                VsHelpers.DTE.Application.Debugger.StepOut();
                e.Action = "Step out";
            }

            e.Handled = true;
        }
    }
}
