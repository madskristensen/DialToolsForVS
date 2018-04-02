using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 2)]
    internal class ZoomControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Zoom.ToString();

        public ZoomControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Zoom\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new ZoomController(host);
        }
    }
}
