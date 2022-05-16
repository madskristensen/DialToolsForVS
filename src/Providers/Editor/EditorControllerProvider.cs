using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    [DialControllerProvider(4)]
    internal class EditorControllerProvider : BaseDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Editor);

        public EditorControllerProvider() { }

        protected override async Task<IDialController> TryCreateControllerAsyncOverride(IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Editor\icon.png");
            var menuItem = await CreateMenuItemAsync(Moniker, iconFilePath);
            var dte = await provider.GetDteAsync(cancellationToken);
            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new EditorController(menuItem, dte, textManager);
        }
    }
}
