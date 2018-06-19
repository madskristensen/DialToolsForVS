using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 5)]
    internal class DebugControllerProvider : IDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Debug);

        public DebugControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Debug\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new DebugController(host);
        }
    }
}
