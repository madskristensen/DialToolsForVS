using System;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class ErrorNavigatorController : IDialController
    {
        private IErrorList _errorList = VsHelpers.DTE.ToolWindows.ErrorList as IErrorList;
        private WindowEvents _events;
        private IDialControllerHost _host;

        public ErrorNavigatorController(IDialControllerHost host)
        {
            _host = host;
            _events = VsHelpers.DTE.Events.WindowEvents;
            _events.WindowActivated += OnWindowActivated;
        }

        private void OnWindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.IsErrorList())
                _host.RequestActivation();
        }

        public Specificity Specificity => Specificity.Global;

        public bool CanHandleClick => false;

        public bool CanHandleRotate
        {
            get { return _errorList.TableControl.Entries.Any(); }
        }

        public void OnClick(RadialControllerButtonClickedEventArgs args, DialEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                Execute("View.NextError");
                e.Action = "Go to next error";
            }
            else
            {
                Execute("View.PreviousError");
                e.Action = "Go to previous error";
            }

            e.Handled = true;
        }

        private static void Execute(string commandName)
        {
            try
            {
                Command command = VsHelpers.DTE.Commands.Item(commandName);

                if (command != null && command.IsAvailable)
                    VsHelpers.DTE.Commands.Raise(command.Guid, command.ID, null, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }
    }
}
