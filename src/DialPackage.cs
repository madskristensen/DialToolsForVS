using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Tasks = System.Threading.Tasks;

namespace DialToolsForVS
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
    internal sealed class DialPackage : AsyncPackage
    {
        protected override Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
            {
                DialControllerHost.Initialize();
            });

            return base.InitializeAsync(cancellationToken, progress);
        }
    }
}