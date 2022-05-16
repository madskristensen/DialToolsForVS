using EnvDTE;

using EnvDTE80;

using Windows.UI.Input;

namespace DialControllerTools
{
    internal class BookmarksController : BaseController
    {
        private readonly Commands _commands;
        private readonly WindowEvents _events;

        public BookmarksController(RadialControllerMenuItem menuItem, DTE2 dte) : base(menuItem)
        {
            _commands = dte.Commands;
            // Switched in provider
#pragma warning disable VSTHRD010 // Invoke single-threaded types on Main thread
            _events = dte.Events.WindowEvents;
            _events.WindowActivated += OnToolWindowActivated;
#pragma warning restore VSTHRD010 // Invoke single-threaded types on Main thread
        }

        private void OnToolWindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.IsBookmarks()) DialPackage.DialControllerHost.RequestActivation(this);
            else if (LostFocus.IsBookmarks()) DialPackage.DialControllerHost.ReleaseActivation();
        }

        public override string Moniker => BookmarksControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override void OnActivate()
        {
            _commands.ExecuteCommand("View.BookmarkWindow");
        }

        public override bool OnClick()
        {
            _commands.ExecuteCommand("Edit.ToggleBookmark");
            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            switch (direction)
            {
                case RotationDirection.Left:
                    _commands.ExecuteCommand("Edit.PreviousBookmark");
                    break;
                case RotationDirection.Right:
                    _commands.ExecuteCommand("Edit.NextBookmark");
                    break;
            }

            return true;
        }
    }
}
