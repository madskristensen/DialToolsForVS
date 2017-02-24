using System.IO;
using System.Reflection;

namespace DialToolsForVS
{
    [DialControllerProvider(4)]
    internal class EditorControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Editor";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string iconFilePath = Path.Combine(folder, "Providers\\Editor\\icon.png");

            host.AddMenuItem(Moniker, iconFilePath);

            return new EditorController();
        }
    }
}
