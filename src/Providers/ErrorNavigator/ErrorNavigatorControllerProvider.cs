using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class ErrorNavigatorControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Error List";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\ErrorNavigator\\icon.png");

            host.AddMenuItem(Moniker, iconFilePath);

            return new ErrorNavigatorController();
        }
    }
}
