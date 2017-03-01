using EnvDTE;

namespace DialToolsForVS
{
    internal class BookmarksController : BaseController
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

        public override string Moniker => BookmarksControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override void OnActivate()
        {
            VsHelpers.ExecuteCommand("View.BookmarkWindow");
        }

        public override bool OnClick()
        {
            VsHelpers.ExecuteCommand("Edit.ToggleBookmark");
            return true;
        }

        public override bool OnRotate(RotationDirection direction)
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
