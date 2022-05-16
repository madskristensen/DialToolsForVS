using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;

namespace DialControllerTools
{
    [DialControllerProvider(order: 2)]
    internal class ZoomControllerProvider : BaseDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Zoom);

        public ZoomControllerProvider() { }

        protected override async Task<IDialController> TryCreateControllerAsyncOverride(IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Zoom\icon.png");
            var menuItem = await CreateMenuItemAsync(Moniker, iconFilePath);
            var dte = await provider.GetDteAsync(cancellationToken);
            IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new ZoomController(menuItem, dte, textManager);
        }
    }
}
