using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 3)]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Navigation.ToString();

        public NavigateControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Navigate\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new NavigateController(host, textManager);
        }
    }
}
