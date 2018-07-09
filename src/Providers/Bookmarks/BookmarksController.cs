using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    internal class BookmarksController : BaseTextController
    {
        private readonly IDialControllerHost _host;
        private readonly Commands _commands;
        private readonly WindowEvents _events;

        public BookmarksController(IDialControllerHost host, IVsTextManager textManager)
            : base(host, textManager)
        {
            _host = host;
            _commands = host.DTE.Commands;
            _events = host.DTE.Events.WindowEvents;
            _events.WindowActivated += WindowActivated;
        }

        private void WindowActivated(Window GotFocus, Window LostFocus)
        {
            if (GotFocus.IsBookmarks())
            {
                _host.ReleaseActivation(LostFocus);
                _host.RequestActivation(GotFocus, Moniker);
            }
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
