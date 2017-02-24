using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class ScrollControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            host.AddMenuItem(PredefinedMonikers.Scroll, Windows.UI.Input.RadialControllerMenuKnownIcon.Scroll);

            return new ScrollController();
        }
    }
}
