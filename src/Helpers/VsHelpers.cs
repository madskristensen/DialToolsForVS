using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using System.ComponentModel.Composition;
using System;

namespace DialToolsForVS
{
    internal static class VsHelpers
    {
        private static IComponentModel _compositionService;
        private static IVsStatusbar statusbar = GetService<SVsStatusbar, IVsStatusbar>();

        public static DTE2 DTE { get; } = GetService<DTE, DTE2>();

        public static TReturnType GetService<TServiceType, TReturnType>()
        {
            return (TReturnType)ServiceProvider.GlobalProvider.GetService(typeof(TServiceType));
        }

        public static void WriteStatus(string text)
        {
            statusbar.FreezeOutput(0);
            statusbar.SetText(text);
            statusbar.FreezeOutput(1);
        }

        ///<summary>Gets the TextView for the active document.</summary>
        public static IWpfTextView GetCurentTextView()
        {
            return GetTextView(GetCurrentNativeTextView());
        }

        public static IWpfTextView GetTextView(IVsTextView nativeView)
        {
            if (nativeView == null)
                return null;

            IVsEditorAdaptersFactoryService editorAdapter = _compositionService.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapter.GetWpfTextView(nativeView);
        }

        public static IVsTextView GetCurrentNativeTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));
            return activeView;
        }

        public static bool IsSolutionExplorer(this Window window)
        {
            return window?.Type == vsWindowType.vsWindowTypeSolutionExplorer;
        }

        public static bool IsErrorList(this Window window)
        {
            return window?.ObjectKind == WindowKinds.vsWindowKindErrorList;
        }

        public static bool IsDocument(this Window window)
        {
            return window?.Kind == "Document";
        }

        public static bool ExecuteCommand(string commandName)
        {
            try
            {
                Command command = DTE.Commands.Item(commandName);

                if (command != null && command.IsAvailable)
                {
                    DTE.Commands.Raise(command.Guid, command.ID, null, null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            return false;
        }

        public static void SatisfyImportsOnce(this object o)
        {
            if (_compositionService == null)
            {
                _compositionService = GetService<SComponentModel, IComponentModel>();
            }

            if (_compositionService != null)
            {
                _compositionService.DefaultCompositionService.SatisfyImportsOnce(o);
            }
        }
    }
}
