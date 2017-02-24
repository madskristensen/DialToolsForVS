namespace DialToolsForVS
{
    internal class BookmarksController : IDialController
    {
        public string Moniker => BookmarksControllerProvider.Moniker;
        public bool CanHandleClick => true;
        public bool CanHandleRotate => true;

        public void OnClick(DialEventArgs e)
        {
            VsHelpers.ExecuteCommand("Edit.ToggleBookmark");
            e.Action = "Toggle bookmark";
            e.Handled = true;
        }

        public void OnRotate(RotationDirection direction, DialEventArgs e)
        {
            if (direction == RotationDirection.Right)
            {
                VsHelpers.ExecuteCommand("Edit.NextBookmark");
                e.Action = "Go to next bookmark";
            }
            else
            {
                VsHelpers.ExecuteCommand("Edit.PreviousBookmark");
                e.Action = "Go to previous bookmark";
            }

            e.Handled = true;
        }
    }
}
