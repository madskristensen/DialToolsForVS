using Microsoft.VisualStudio.Shell;
using System.Linq;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class ErrorsController : IDialController
    {
        private IErrorList _errorList = VsHelpers.DTE.ToolWindows.ErrorList as IErrorList;

        public string Moniker => ErrorsControllerProvider.Moniker;
        public Specificity Specificity => Specificity.Global;
        public bool CanHandleClick => true;

        public bool CanHandleRotate
        {
            get { return _errorList.TableControl.Entries.Any(); }
        }

        public void OnClick(RadialControllerButtonClickedEventArgs args, DialEventArgs e)
        {
            VsHelpers.DTE.ToolWindows.ErrorList.Parent?.Activate();
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("View.NextError");
                e.Action = "Go to next error";
            }
            else
            {
                VsHelpers.ExecuteCommand("View.PreviousError");
                e.Action = "Go to previous error";
            }

            e.Handled = true;
        }
    }
}
