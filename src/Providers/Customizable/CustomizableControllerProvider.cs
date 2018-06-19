using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 9)]
    internal class CustomizableControllerProvider : IDialControllerProvider
    {
        public static string Moniker = nameof(KnownProviders.Customizable);

        public CustomizableControllerProvider() { }

        public async Task<IDialController> TryCreateControllerAsync(IDialControllerHost host, IAsyncServiceProvider provider, CancellationToken cancellationToken)
        {
            string iconFilePath = VsHelpers.GetFileInVsix(@"Providers\Customizable\icon.png");
            await host.AddMenuItemAsync(Moniker, iconFilePath);

            return new CustomizableController(host);
        }
    }
}
