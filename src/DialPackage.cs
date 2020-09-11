using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Tasks = System.Threading.Tasks;

namespace DialControllerTools
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
            async Tasks.Task InitializeDialControllerHostAsync()
            {
#pragma warning disable U2U1003 // Avoid declaring methods used in delegate constructors static
                void LoadOptions()
                {
                    Options = (Options)GetDialogPage(typeof(Options));
                    CustomOptions = (CustomOptions)GetDialogPage(typeof(CustomOptions));
                }
#pragma warning restore U2U1003 // Avoid declaring methods used in delegate constructors static
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
            }

            KnownUIContexts.ShellInitializedContext.WhenActivated(() => ThreadHelper.JoinableTaskFactory.StartOnIdle(InitializeDialControllerHostAsync));
            await base.InitializeAsync(cancellationToken, progress);
        }
    }
}