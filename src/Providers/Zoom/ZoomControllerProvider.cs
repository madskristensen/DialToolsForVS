using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 2)]
    internal class ZoomControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Zoom";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Zoom\\icon.png");

            host.AddMenuItem(Moniker, iconFilePath);

            return new ZoomController();
        }
    }
}
