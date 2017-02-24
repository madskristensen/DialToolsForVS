using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            host.AddMenuItem(PredefinedMonikers.Navigate, Windows.UI.Input.RadialControllerMenuKnownIcon.NextPreviousTrack);

            return new NavigateController();
        }
    }
}
