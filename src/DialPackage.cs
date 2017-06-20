using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Tasks = System.Threading.Tasks;

namespace DialToolsForVS
{
    [Guid(PackageGuids.guidPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Surface Dial", "General", 0, 0, true, ProvidesLocalizedCategoryName = false)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string)]
    internal sealed class DialPackage : AsyncPackage
    {
        public static Options Options
        {
            get;
            private set;
        }

        protected override Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            Options = (Options)GetDialogPage(typeof(Options));

            ThreadHelper.Generic.BeginInvoke(DispatcherPriority.ApplicationIdle, () =>
            {
                try
                {
                    DialControllerHost.Initialize();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });

            return base.InitializeAsync(cancellationToken, progress);
        }
    }
}