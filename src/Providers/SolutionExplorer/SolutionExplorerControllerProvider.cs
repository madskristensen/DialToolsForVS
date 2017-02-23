using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    public class SolutionExplorerControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController()
        {
            return new SolutionExplorerController();
        }
    }
}
