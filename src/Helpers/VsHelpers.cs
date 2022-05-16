
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;

namespace DialControllerTools
{
    internal static class VsHelpers
    {
        internal static async Task<TReturnType> GetServiceAsync<TServiceType, TReturnType>(this IAsyncServiceProvider provider, CancellationToken cancellationToken)
         => (TReturnType)await provider.GetServiceAsync(typeof(TServiceType));

        internal static Task<DTE2> GetDteAsync(this IAsyncServiceProvider provider, CancellationToken cancellationToken)
         => provider.GetServiceAsync<DTE, DTE2>(cancellationToken);

        public static string GetFileInVsix(string relativePath)
        {
            string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(folder, relativePath);
        }

        public static bool IsSolutionExplorer(this Window window)
        {
            return IsTool(window) && window.Type == vsWindowType.vsWindowTypeSolutionExplorer;
        }

        public static bool IsErrorList(this Window window)
        {
            return IsTool(window) && window.ObjectKind == WindowKinds.vsWindowKindErrorList;
        }

        public static bool IsBookmarks(this Window window)
        {
            return IsTool(window) && window.ObjectKind == WindowKinds.vsWindowKindBookmarks;
        }

        public static bool IsDocument(this Window window) => window?.Kind == "Document";
        public static bool IsTool(this Window window) => window?.Kind == "Tool";

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
    }
}
