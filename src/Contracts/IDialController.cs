using Windows.UI.Input;

namespace DialToolsForVS
{
    public interface IDialController
    {
        string Moniker { get; }
        Specificity Specificity { get; }
        bool CanHandleClick { get; }
        bool CanHandleRotate { get; }
        void OnClick(DialEventArgs e);
        void OnRotate(RotationDirection direction, DialEventArgs e);
    }
}
