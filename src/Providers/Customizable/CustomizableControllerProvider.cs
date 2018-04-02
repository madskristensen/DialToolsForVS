using System.Threading;
using System.Threading.Tasks;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 9)]
    internal class CustomizableControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Customizable.ToString();

        public CustomizableControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Customizable\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new CustomizableController(host);
        }
    }
}
