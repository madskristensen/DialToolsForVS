using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 6)]
    internal class ErrorsControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Errors";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Errors\\icon.png");

            host.AddMenuItem(Moniker, iconFilePath);

            return new ErrorsController(host);
        }
    }
}
