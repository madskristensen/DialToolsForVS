namespace DialToolsForVS
{
    [DialControllerProvider(order: 7)]
    internal class BookmarksControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Bookmarks.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        public BookmarksControllerProvider() { }
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Bookmarks\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new BookmarksController(host);
        }
    }
}
