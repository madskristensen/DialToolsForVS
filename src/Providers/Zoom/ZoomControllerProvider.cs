using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class ZoomControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            host.AddMenuItem(PredefinedMonikers.Zoom, Windows.UI.Input.RadialControllerMenuKnownIcon.Zoom);

            return new ZoomController();
        }
    }
}
