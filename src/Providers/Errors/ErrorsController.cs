using System.Linq;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;

using Windows.UI.Input;

namespace DialControllerTools
{
    internal class ErrorsController : BaseController
    {
        private readonly DTE2 _dte;
        private readonly IErrorList _errorList;
        private readonly WindowEvents _events;
        public override string Moniker => ErrorsControllerProvider.Moniker;

        public override bool CanHandleClick => true;

        public override bool CanHandleRotate => _errorList.TableControl.Entries.Any();


        public ErrorsController(RadialControllerMenuItem menuItem, DTE2 dte) : base(menuItem)
        {
            _dte = dte;
            // Switched in provider
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            _errorList = _dte.ToolWindows.ErrorList as IErrorList;
            _events = _dte.Events.WindowEvents;
            _events.WindowActivated += OnToolWindowActivated;
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }

        private void OnToolWindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.IsErrorList() && _errorList.TableControl.Entries.Any())
                DialPackage.DialControllerHost.RequestActivation(this);
            else if (LostFocus.IsErrorList())
                DialPackage.DialControllerHost.ReleaseActivation();
        }

        // Radial Controller events always occur on the UI thread
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
        public override void OnActivate() => _dte.ToolWindows.ErrorList.Parent?.Activate();
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread

        public override bool OnClick()
        {
            OnActivate();
            return true;
        }

        public override bool OnRotate(RotationDirection direction)
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
