using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    [DialControllerProvider(order: 1)]
    internal class ScrollControllerProvider : BaseDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Scroll);

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public ScrollControllerProvider() { }

        protected override async Task<IDialController> TryCreateControllerAsyncOverride(IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Scroll\icon.png");
            var menuItem = await CreateMenuItemAsync(Moniker, iconFilePath);
            var dte = await provider.GetDteAsync(cancellationToken);
            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new ScrollController(menuItem, dte, textManager);
        }
    }
}
