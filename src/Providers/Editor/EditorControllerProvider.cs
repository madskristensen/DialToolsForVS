using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;

namespace DialToolsForVS
{
    [DialControllerProvider(4)]
    internal class EditorControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Editor.ToString();

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public EditorControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Editor\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new EditorController(host, CompletionBroker);
        }
    }
}
