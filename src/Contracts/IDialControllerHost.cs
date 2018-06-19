using System.Collections.Generic;
using System.Threading.Tasks;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Windows.UI.Input;

namespace DialToolsForVS
{
    public interface IDialControllerHost
    {
        DTE2 DTE { get; }
        IComponentModel CompositionService { get; }
        Task AddMenuItemAsync(string moniker, string iconFilePath);
        void RemoveMenuItem(string moniker);
        void RequestActivation(RadialController controller, string moniker);
        void RequestActivation(EnvDTE.Window window, string moniker);
        void ReleaseActivation(RadialController controller);
        void ReleaseActivation(EnvDTE.Window window);
    }
}
