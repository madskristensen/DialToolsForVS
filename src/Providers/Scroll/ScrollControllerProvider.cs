using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;

namespace DialToolsForVS
{
    [DialControllerProvider(order: 1)]
    internal class ScrollControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Scroll.ToString();

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Scroll\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new ScrollController(CompletionBroker);
        }
    }
}
