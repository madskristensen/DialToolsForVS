using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    public class NumbersControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController()
        {
            return new NumbersController();
        }
    }
}
