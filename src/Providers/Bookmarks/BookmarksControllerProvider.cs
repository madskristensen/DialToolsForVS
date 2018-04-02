using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 7)]
    internal class BookmarksControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Bookmarks.ToString();

        public BookmarksControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Bookmarks\\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new BookmarksController(host);
        }
    }
}
