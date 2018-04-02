using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 6)]
    internal class ErrorsControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Errors.ToString();

        public ErrorsControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Errors\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new ErrorsController(host);
        }
    }
}
