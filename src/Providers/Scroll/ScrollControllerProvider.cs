using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 1)]
    internal class ScrollControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Scroll.ToString();

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public ScrollControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Scroll\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new ScrollController(host, CompletionBroker);
        }
    }
}
