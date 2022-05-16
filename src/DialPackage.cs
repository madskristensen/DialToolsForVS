using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

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

        public static DialControllerHost DialControllerHost { get; private set; }


        private async Task<TReturnType> GetServiceAsync<TServiceType, TReturnType>(CancellationToken cancellationToken)
         => (TReturnType)await GetServiceAsync(typeof(TServiceType));

        internal static Task<OutputWindowPane> GetOutputPaneAsync() => OutputWindowPane.CreateAsync(Vsix.Name);

        protected override async Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            async Task InitializeDialControllerHostAsync()
            {
                void LoadOptions()
                {
                    Options = (Options)GetDialogPage(typeof(Options));
                    CustomOptions = (CustomOptions)GetDialogPage(typeof(CustomOptions));
                }
                var optionsLoadTask = ThreadHelper.JoinableTaskFactory.StartOnIdle(LoadOptions);

                try
                {
                    var serviceProvider = await GetServiceAsync<SAsyncServiceProvider, IAsyncServiceProvider>(cancellationToken);
                    var outputPaneTask = GetOutputPaneAsync();
                    var controllersTask = serviceProvider.GetControllersAsync(cancellationToken);
                    await Task.WhenAll(outputPaneTask, controllersTask, optionsLoadTask.Task);
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

#pragma warning disable VSTHRD103 // Call async methods when in an async method
                    DialControllerHost = new DialControllerHost(outputPaneTask.Result, controllersTask.Result);
#pragma warning restore VSTHRD103 // Call async methods when in an async method
                    Options.OptionsApplied += DialControllerHost.OptionsApplied;
                }
                catch (Exception ex)
                {
                    await ex.LogAsync("DialControllerHost.InitializeAsync");
                }
            }

            KnownUIContexts.ShellInitializedContext.WhenActivated(() => ThreadHelper.JoinableTaskFactory.StartOnIdle(InitializeDialControllerHostAsync));
            await base.InitializeAsync(cancellationToken, progress);
        }
    }
}