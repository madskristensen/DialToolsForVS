using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class DebugControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            return new DebugController(host);
        }
    }
}
