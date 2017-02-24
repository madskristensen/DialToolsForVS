using Microsoft.VisualStudio.Shell;
using System.Linq;

namespace DialToolsForVS
{
    internal class ErrorsController : IDialController
    {
        private IErrorList _errorList = VsHelpers.DTE.ToolWindows.ErrorList as IErrorList;

        public string Moniker => ErrorsControllerProvider.Moniker;
        public bool CanHandleClick => true;

        public bool CanHandleRotate
        {
            get { return _errorList.TableControl.Entries.Any(); }
        }

        public bool OnClick()
        {
            VsHelpers.DTE.ToolWindows.ErrorList.Parent?.Activate();
            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("View.NextError");
            }
            else
            {
                VsHelpers.ExecuteCommand("View.PreviousError");
            }

            return true;
        }
    }
}
