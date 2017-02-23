using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Tasks = System.Threading.Tasks;
using System.Windows.Threading;

namespace DialToolsForVS
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
    internal sealed class DialPackage : Package
    {
        protected override void Initialize()
        {
            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
            {
                DialControllerHost.Initialize();
            });
        }
    }
}