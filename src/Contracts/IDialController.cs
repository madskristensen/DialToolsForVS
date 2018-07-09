using Windows.UI.Input;

namespace DialControllerTools
{
    public interface IDialController
    {
        string Moniker { get; }
        bool CanHandleClick { get; }
        bool CanHandleRotate { get; }
        bool OnClick();
        bool OnRotate(RotationDirection direction);
        void OnActivate();
    }
}
