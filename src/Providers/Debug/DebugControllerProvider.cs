using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 5)]
    internal class DebugControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Debug.ToString();

        public DebugControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Debug\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new DebugController(host);
        }
    }
}
