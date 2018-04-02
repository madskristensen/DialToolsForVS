using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 3)]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Navigation.ToString();

        public NavigateControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Navigate\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new NavigateController(host);
        }
    }
}
