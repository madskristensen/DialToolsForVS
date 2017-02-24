using System.ComponentModel.Composition;
using Windows.UI.Input;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            host.AddMenuItem(PredefinedMonikers.Navigate, RadialControllerMenuKnownIcon.NextPreviousTrack);

            return new NavigateController();
        }
    }
}
