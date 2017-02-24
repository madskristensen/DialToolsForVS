using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class ScrollControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Scroll\\icon.png");

            host.AddMenuItem(PredefinedMonikers.Scroll, iconFilePath);

            return new ScrollController();
        }
    }
}
