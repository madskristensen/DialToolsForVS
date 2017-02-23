using Windows.UI.Input;

namespace DialToolsForVS
{
    public interface IDialController
    {
        Specificity Specificity { get; }
        bool CanHandleClick { get; }
        bool CanHandleRotate { get; }
        bool OnClick(RadialControllerButtonClickedEventArgs args);
        bool OnRotate(RotationDirection direction);
    }
}
