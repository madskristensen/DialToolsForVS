using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class ErrorNavigatorControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            return new ErrorNavigatorController(host);
        }
    }
}
