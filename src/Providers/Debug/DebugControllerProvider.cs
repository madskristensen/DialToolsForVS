using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    public class DebugControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController()
        {
            return new DebugController();
        }
    }
}
