
using Windows.UI.Input;

namespace DialControllerTools
{
    public abstract class BaseController : IDialController
    {
        public abstract string Moniker { get; }

        public RadialControllerMenuItem MenuItem { get; }

        public virtual bool CanHandleClick => false;

        public virtual bool CanHandleRotate => false;

        public virtual bool IsHapticFeedbackEnabled => true;

        public BaseController(RadialControllerMenuItem menuItem)
        {
            MenuItem = menuItem;
            menuItem.Invoked += (sender, args) => OnActivate();
        }

        public virtual void OnActivate()
        { }

        public virtual bool OnClick()
        {
            return false;
        }

        public virtual bool OnRotate(RotationDirection direction)
        {
            return false;
        }
    }
}
