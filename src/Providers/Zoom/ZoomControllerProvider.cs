using System.ComponentModel.Composition;
using Windows.UI.Input;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class ZoomControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            host.AddMenuItem(PredefinedMonikers.Zoom, RadialControllerMenuKnownIcon.Zoom);

            return new ZoomController();
        }
    }
}
