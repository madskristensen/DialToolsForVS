using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialToolsForVS
{
    [DialControllerProvider(4)]
    internal class EditorControllerProvider : IDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Editor);

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public EditorControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Editor\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new EditorController(host, textManager, CompletionBroker);
        }
    }
}
