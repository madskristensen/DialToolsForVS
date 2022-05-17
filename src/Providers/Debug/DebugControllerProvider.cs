using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

namespace DialControllerTools
{
    [DialControllerProvider(order: 5)]
    internal class DebugControllerProvider : BaseDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Debug);

        public DebugControllerProvider() { }

        protected override async Task<IDialController> TryCreateControllerAsyncOverride(IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Debug\icon.png");
            var menuItem = await CreateMenuItemAsync(Moniker, iconFilePath);
            var dte = await provider.GetDteAsync(cancellationToken);
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            return new DebugController(menuItem, dte);
        }
    }
}
