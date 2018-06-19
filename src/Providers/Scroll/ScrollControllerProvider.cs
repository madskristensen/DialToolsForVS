using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 1)]
    internal class ScrollControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Scroll.ToString();

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public ScrollControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Scroll\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new ScrollController(host, textManager, CompletionBroker);
        }
    }
}
