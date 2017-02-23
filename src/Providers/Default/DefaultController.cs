using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;
using Windows.UI.Input;

namespace DialToolsForVS
{
    internal class DefaultController : IDialController
    {
        public Specificity Specificity => Specificity.None;

        public bool CanHandleClick => true;
        public bool CanHandleRotate => true;

        public bool OnClick(RadialControllerButtonClickedEventArgs args)
        {
            SendKeys.Send("{ENTER}");

            return true;
        }

        public bool OnRotate(RotationDirection direction)
        {
            if (direction == RotationDirection.Right)
            {
                SendKeys.Send("{DOWN}");
            }
            else
            {
                SendKeys.Send("{UP}");
            }

            return true;
        }
    }
}
