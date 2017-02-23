using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    internal class ColorControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController(IDialControllerHost host)
        {
            return new ColorController();
        }
    }
}
