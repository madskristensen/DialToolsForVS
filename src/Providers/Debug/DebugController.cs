using EnvDTE;
using EnvDTE80;
using Windows.UI.Input;

namespace DialToolsForVS
{
    public class DebugController : IDialController
    {
        private DTE2 _dte = VsHelpers.DTE;

        public Specificity Specificity => Specificity.IdeState;

        public bool CanHandleClick
        {
            get { return _dte.Application?.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode; }
        }

        public bool CanHandleRotate
        {
            get { return CanHandleClick; }
        }

        public bool OnClick(RadialControllerButtonClickedEventArgs args)
        {
            _dte.Application.Debugger.StepInto();

            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                _dte.Application.Debugger.StepOver();
            }
            else
            {
                _dte.Application.Debugger.StepOut();
            }

            return true;
        }
    }
}
