using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 6)]
    internal class ErrorsControllerProvider : IDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Errors);

        public ErrorsControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Errors\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new ErrorsController(host);
        }
    }
}
