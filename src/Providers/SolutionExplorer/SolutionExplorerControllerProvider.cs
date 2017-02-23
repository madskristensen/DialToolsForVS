using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class SolutionExplorerControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            return new SolutionExplorerController();
        }
    }
}
