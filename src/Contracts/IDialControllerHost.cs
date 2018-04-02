using System.Threading.Tasks;
using EnvDTE80;
using Windows.UI.Input;

namespace DialToolsForVS
{
    public interface IDialControllerHost
    {
        DTE2 DTE { get; }
        Task AddMenuItemAsync(string moniker, string iconFilePath);
        void RemoveMenuItem(string moniker);
        void RequestActivation(string moniker);
        void ReleaseActivation();
    }
}
