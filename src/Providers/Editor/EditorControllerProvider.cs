using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;

namespace DialToolsForVS
{
    [DialControllerProvider(4)]
    internal class EditorControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Editor.ToString();

        [Import]
        private ICompletionBroker CompletionBroker { get; set; }

        public IDialController TryCreateController(IDialControllerHost host)
        public EditorControllerProvider() { }
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Editor\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new EditorController(CompletionBroker);
        }
    }
}
