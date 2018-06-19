using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 7)]
    internal class BookmarksControllerProvider : IDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Bookmarks);

        public BookmarksControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Bookmarks\\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new BookmarksController(host, textManager);
        }
    }
}
