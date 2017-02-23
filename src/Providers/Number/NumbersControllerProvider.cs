using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class NumbersControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            return new NumbersController();
        }
    }
}
