using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    [DialControllerProvider(order: 8)]
    internal class FindControllerProvider : IDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Find);

        public FindControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Find\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new FindController(host, textManager);
        }
    }
}
