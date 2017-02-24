using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using Windows.UI.Input;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Navigate\\icon.png");

            host.AddMenuItem(PredefinedMonikers.Navigate, iconFilePath);

            return new NavigateController();
        }
    }
}
