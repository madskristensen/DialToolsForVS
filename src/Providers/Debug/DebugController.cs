using EnvDTE;

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
        public bool CanHandleClick => true;
        public bool CanHandleRotate
        {
            get { return VsHelpers.DTE.Application?.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode; }
        }

        public bool OnClick()
        {
            dbgDebugMode? debugMode = VsHelpers.DTE.Application?.Debugger.CurrentMode;

            if (debugMode == dbgDebugMode.dbgBreakMode)
            {
                VsHelpers.DTE.Application.Debugger.StepInto();
            }
            else if (debugMode == dbgDebugMode.dbgDesignMode)
            {
                VsHelpers.ExecuteCommand("Debug.ToggleBreakpoint");
            }

            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.DTE.Application.Debugger.StepOver();
            }
            else
            {
                VsHelpers.DTE.Application.Debugger.StepOut();
            }

            return true;
        }
    }
}
