using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 2)]
    internal class ZoomControllerProvider : IDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Zoom);

        public ZoomControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Zoom\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new ZoomController(host, textManager);
        }
    }
}
