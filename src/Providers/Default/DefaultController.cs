using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Forms;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class DefaultController : IDialController
    {
        public string Moniker => PredefinedMonikers.Scroll;
        public Specificity Specificity => (Specificity)int.MaxValue;
        public bool CanHandleClick => true;
        public bool CanHandleRotate => true;

        public void OnClick(RadialControllerButtonClickedEventArgs args, DialEventArgs e)
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

            e.Handled = true;
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            IWpfTextView view = VsHelpers.GetCurentTextView();

            if (view != null && view.HasAggregateFocus)
            {
                string cmd = direction == RotationDirection.Left ? "Edit.ScrollLineUp" : "Edit.ScrollLineDown";
                VsHelpers.ExecuteCommand(cmd);
            }
            else
            {
                string key = direction == RotationDirection.Left ? "{UP}" : "{DOWN}";
                SendKeys.Send(key);
            }

            e.Handled = true;
        }
    }
}
