using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

namespace DialControllerTools
{
    [DialControllerProvider(order: 8)]
    internal class FindControllerProvider : BaseDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Find);

        public FindControllerProvider() { }

        protected override async Task<IDialController> TryCreateControllerAsyncOverride(IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Find\icon.png");
            var menuItem = await CreateMenuItemAsync(Moniker, iconFilePath);
            var dte = await provider.GetDteAsync(cancellationToken);
            //IVsTextManager textManager = await provider.GetServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
            return new FindController(menuItem, dte);
        }
    }
}
