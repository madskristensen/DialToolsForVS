namespace DialToolsForVS
{
    [DialControllerProvider(order: 7)]
    internal class BookmarksControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Bookmarks";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Bookmarks\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new BookmarksController(host);
        }
    }
}
