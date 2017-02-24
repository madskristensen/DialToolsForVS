namespace DialToolsForVS
{
    internal class BookmarksController : IDialController
    {
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
