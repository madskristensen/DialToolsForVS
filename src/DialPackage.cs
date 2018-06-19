using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Windows.UI.Input;
using Tasks = System.Threading.Tasks;

namespace DialToolsForVS
{
    [Guid(PackageGuids.guidPackageString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Surface Dial", "General", 0, 0, true, ProvidesLocalizedCategoryName = false)]
    [ProvideOptionPage(typeof(CustomOptions), "Surface Dial", "Custom controls", 0, 0, true, ProvidesLocalizedCategoryName = false)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    internal sealed class DialPackage : AsyncPackage
    {
        public static Options Options
        {
            get;
            private set;
        }

        public static CustomOptions CustomOptions
        {
            get;
            private set;
        }

        private async Task<TReturnType> GetServiceAsync<TServiceType, TReturnType>(CancellationToken cancellationToken)
         => (TReturnType)await GetServiceAsync(typeof(TServiceType));

        protected override async Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            void LoadOptions()
            {
                Options = (Options)GetDialogPage(typeof(Options));
                CustomOptions = (CustomOptions)GetDialogPage(typeof(CustomOptions));
            }
            var optionsLoadTask = ThreadHelper.JoinableTaskFactory.StartOnIdle(LoadOptions);

            Logger.Initialize(await GetServiceAsync<SVsOutputWindow, IVsOutputWindow>(cancellationToken));
            try
            {
                var serviceProvider = await GetServiceAsync<SAsyncServiceProvider, Microsoft.VisualStudio.Shell.IAsyncServiceProvider>(cancellationToken);

                await DialControllerHost.InitializeAsync(serviceProvider, optionsLoadTask, cancellationToken);
                Options.OptionsApplied += DialControllerHost.Instance.OptionsApplied;
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex);
            }

            await base.InitializeAsync(cancellationToken, progress);
        }
    }
}