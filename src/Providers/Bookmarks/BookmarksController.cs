using EnvDTE;

namespace DialToolsForVS
{
    internal class BookmarksController : IDialController
    {
        private WindowEvents _events;
        private IDialControllerHost _host;

        public BookmarksController(IDialControllerHost host)
        {
            _host = host;
            _events = VsHelpers.DTE.Events.WindowEvents;
            _events.WindowActivated += WindowActivated;
        }

        private void WindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.IsBookmarks())
            {
                _host.RequestActivation(Moniker);
            }
        }

        public string Moniker => BookmarksControllerProvider.Moniker;
        public bool CanHandleClick => true;
        public bool CanHandleRotate => true;

        public bool OnClick()
        {
            VsHelpers.ExecuteCommand("Edit.ToggleBookmark");
            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("Edit.NextBookmark");
            }
            else
            {
                VsHelpers.ExecuteCommand("Edit.PreviousBookmark");
            }

            return true;
        }
    }
}
