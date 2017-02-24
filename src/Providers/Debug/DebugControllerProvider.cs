using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 5)]
    internal class DebugControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Debug";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Debug\\icon.png");

            host.AddMenuItem(Moniker, iconFilePath);

            return new DebugController(host);
        }
    }
}
