using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio;

namespace DialToolsForVS
{
    public static class VsHelpers
    {
        private static IVsUIShell5 _shell = GetService<SVsUIShell, IVsUIShell5>();
        private static IVsSolution5 _solution = GetService<IVsSolution, IVsSolution5>();
        private static IComponentModel _compositionService;

        internal static DTE2 DTE { get; } = GetService<DTE, DTE2>();

        public static TReturnType GetService<TServiceType, TReturnType>()
        {
            return (TReturnType)ServiceProvider.GlobalProvider.GetService(typeof(TServiceType));
        }

        /// <summary>Gets the root folder of any Visual Studio project.</summary>
        public static string GetRootFolder(this Project project)
        {
            if (project == null)
                return null;

            if (project.IsKind(ProjectKinds.vsProjectKindSolutionFolder))
                return Path.GetDirectoryName(DTE.Solution.FullName);

            if (string.IsNullOrEmpty(project.FullName))
                return null;

            string fullPath;

            try
            {
                fullPath = project.Properties.Item("FullPath").Value as string;
            }
            catch (ArgumentException)
            {
                try
                {
                    // MFC projects don't have FullPath, and there seems to be no way to query existence
                    fullPath = project.Properties.Item("ProjectDirectory").Value as string;
                }
                catch (ArgumentException)
                {
                    // Installer projects have a ProjectPath.
                    fullPath = project.Properties.Item("ProjectPath").Value as string;
                }
            }

            if (string.IsNullOrEmpty(fullPath))
                return File.Exists(project.FullName) ? Path.GetDirectoryName(project.FullName) : null;

            if (Directory.Exists(fullPath))
                return fullPath;

            if (File.Exists(fullPath))
                return Path.GetDirectoryName(fullPath);

            return null;
        }

        public static ITextBuffer GetCurentTextBuffer()
        {
            //TODO: Get active ProjectionBuffer
            return GetCurentTextView().TextBuffer;
        }

        ///<summary>Gets the TextView for the active document.</summary>
        public static IWpfTextView GetCurentTextView()
        {
            IVsEditorAdaptersFactoryService editorAdapter = _compositionService.GetService<IVsEditorAdaptersFactoryService>();

            return editorAdapter.GetWpfTextView(GetCurrentNativeTextView());
        }

        public static IVsTextView GetCurrentNativeTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));

            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out IVsTextView activeView));
            return activeView;
        }

        public static bool IsSolutionExplorer(this Window window)
        {
            return window.Type == vsWindowType.vsWindowTypeSolutionExplorer;
        }

        public static bool IsErrorList(this Window window)
        {
            return window.ObjectKind == WindowKinds.vsWindowKindErrorList;
        }

        public static bool IsKind(this Project project, params string[] kindGuids)
        {
            foreach (string guid in kindGuids)
            {
                if (project.Kind.Equals(guid, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        public static IServiceProvider AsServiceProvider(this DTE2 dte)
        {
            return new ServiceProvider(dte as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
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

    public static class ProjectTypes
    {
        public const string ASPNET_5 = "{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}";
        public const string WEBSITE_PROJECT = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
        public const string UNIVERSAL_APP = "{262852C6-CD72-467D-83FE-5EEB1973A190}";
        public const string NODE_JS = "{9092AA53-FB77-4645-B42D-1CCCA6BD08BD}";
        public const string SSDT = "{00d1a9c2-b5f0-4af3-8072-f6c62b433612}";
    }
}
