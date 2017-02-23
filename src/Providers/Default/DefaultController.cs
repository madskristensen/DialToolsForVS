using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Forms;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class DefaultController : IDialController
    {
        public Specificity Specificity => (Specificity)int.MaxValue;
        public bool CanHandleClick => true;
        public bool CanHandleRotate => true;

        public bool OnClick(RadialControllerButtonClickedEventArgs args)
        {
            if (VsHelpers.DTE.ActiveWindow.IsDocument())
            {
                IWpfTextView view = VsHelpers.GetCurentTextView();

                if (view != null && view.HasAggregateFocus)
                    SendKeys.Send("+{F10}");
                else
                    SendKeys.Send("{ENTER}");
            }
            else
            {
                SendKeys.Send("{ENTER}");
            }

            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                SendKeys.Send("{DOWN}");
            }
            else
            {
                SendKeys.Send("{UP}");
            }

            return true;
        }
    }
}
