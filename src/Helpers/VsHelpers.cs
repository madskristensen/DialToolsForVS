using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using ServiceProvider = Microsoft.VisualStudio.Shell.ServiceProvider;
using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

namespace DialToolsForVS
{
    internal static class VsHelpers
    {
        private static IComponentModel _compositionService;

        public static Task<DTE2> GetDteAsync(CancellationToken cancellationToken) => GetServiceAsync<DTE, DTE2>(cancellationToken);

        public static async Task<TReturnType> GetServiceAsync<TServiceType, TReturnType>(CancellationToken cancellationToken = default)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            return (TReturnType)ServiceProvider.GlobalProvider.GetService(typeof(TServiceType));
        }

        public static async Task WriteStatusAsync(string text)
        {
            var statusbar = await GetServiceAsync<SVsStatusbar, IVsStatusbar>();
            statusbar.FreezeOutput(0);
            statusbar.SetText(text);
            statusbar.FreezeOutput(1);
        }

        ///<summary>Gets the TextView for the active document.</summary>
        public static IWpfTextView GetCurrentTextView()
         => ThreadHelper.JoinableTaskFactory.Run(async () => GetTextView(await GetCurrentNativeTextViewAsync()));

        public static IWpfTextView GetTextView(IVsTextView nativeView)
        {
            if (nativeView == null)
                return null;

            IVsEditorAdaptersFactoryService editorAdapter = _compositionService.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapter.GetWpfTextView(nativeView);
        }

        public static async Task<IVsTextView> GetCurrentNativeTextViewAsync()
        {
            var textManager = await GetServiceAsync<SVsTextManager, IVsTextManager>();

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));
            return activeView;
        }

        public static string GetFileInVsix(string relativePath)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(folder, relativePath);
        }

        public static bool IsSolutionExplorer(this Window window)
        {
            return window?.Type == vsWindowType.vsWindowTypeSolutionExplorer;
        }

        public static bool IsErrorList(this Window window)
        {
            return window?.ObjectKind == WindowKinds.vsWindowKindErrorList;
        }

        public static bool IsBookmarks(this Window window)
        {
            return window?.ObjectKind == WindowKinds.vsWindowKindBookmarks;
        }

        public static bool IsDocument(this Window window)
        {
            return window?.Kind == "Document";
        }

        public static bool ExecuteCommand(this Commands commands, string commandName)
        {
            try
            {
                Command command = commands.Item(commandName);

                if (command != null && command.IsAvailable)
                {
                    commands.Raise(command.Guid, command.ID, null, null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            return false;
        }

        public static async Task SatisfyImportsOnceAsync(this object o, CancellationToken cancellationToken = default)
        {
            if (_compositionService == null)
            {
                _compositionService = await GetServiceAsync<SComponentModel, IComponentModel>(cancellationToken);
            }

            if (_compositionService != null)
            {
                _compositionService.DefaultCompositionService.SatisfyImportsOnce(o);
            }
        }
    }
}
