using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 3)]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Navigation";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Navigate\\icon.png");

            host.AddMenuItem(Moniker, iconFilePath);

            return new NavigateController();
        }
    }
}
