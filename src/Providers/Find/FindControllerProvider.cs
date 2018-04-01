using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 8)]
    internal class FindControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Find.ToString();

        public FindControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Find\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new FindController(host);
        }
    }
}
