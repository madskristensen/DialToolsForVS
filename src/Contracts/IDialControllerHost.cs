using Windows.UI.Input;

namespace DialToolsForVS
{
    public interface IDialControllerHost
    {
        void AddMenuItem(string moniker, string iconFilePath);
        void AddMenuItem(string moniker, RadialControllerMenuKnownIcon icon);
        void RemoveMenuItem(string moniker);
        void RequestActivation(string moniker);
        void ReleaseActivation();
    }
}
