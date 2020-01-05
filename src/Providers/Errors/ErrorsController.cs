using Microsoft.VisualStudio.Shell;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace DialControllerTools
{
    internal class ErrorsController : IDialController
    {
        private readonly IDialControllerHost _host;
        private readonly DTE2 _dte;
        private readonly IErrorList _errorList;
        private readonly WindowEvents _events;

        public ErrorsController(IDialControllerHost host)
        {
            _host = host;
            _dte = host.DTE;
            _errorList = _dte.ToolWindows.ErrorList as IErrorList;
            _events = _dte.Events.WindowEvents;
            _events.WindowActivated += WindowActivated;
        }

        private void WindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.IsErrorList() && _errorList.TableControl.Entries.Any())
            {
                _host.ReleaseActivation(LostFocus.LinkedWindowFrame);
                _host.RequestActivation(GotFocus.LinkedWindowFrame, Moniker);
            }
        }

        public string Moniker => ErrorsControllerProvider.Moniker;

        public bool CanHandleClick => true;

        public bool CanHandleRotate
        {
            get { return _errorList.TableControl.Entries.Any(); }
        }

        public void OnActivate()
        {
            _dte.ToolWindows.ErrorList.Parent?.Activate();
        }

        public bool OnClick()
        {
            OnActivate();
            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            var commands = _dte.Commands;
            switch (direction)
            {
                case RotationDirection.Left:
                    commands.ExecuteCommand("View.PreviousError");
                    break;
                case RotationDirection.Right:
                    commands.ExecuteCommand("View.NextError");
                    break;
            }

            return true;
        }
    }
}
