using Windows.UI.Input;

namespace DialToolsForVS
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
