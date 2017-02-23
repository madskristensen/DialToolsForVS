using System.ComponentModel.Composition;

namespace DialToolsForVS
{
    [Export(typeof(IDialControllerProvider))]
    public class ColorControllerProvider : IDialControllerProvider
    {
        public IDialController TryCreateController()
        {
            return new ColorController();
        }
    }
}
