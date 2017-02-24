using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 1)]
    internal class ScrollControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Scroll";
        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Scroll\\icon.png");

            host.AddMenuItem(Moniker, iconFilePath);

            return new ScrollController();
        }
    }
}
