using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DialControllerTools.Helpers
{
    public static class VsCommands
    {
        internal static ImmutableArray<string> ParseCommands(string commandsString) =>
            commandsString
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToImmutableArray();

        internal static string ReadCommandsAsString()
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "DialControllerTools.Resources.commands.txt";
            var stream = assembly.GetManifestResourceStream(resourceName);
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [Conditional("Debug")]
        internal static void CheckEmptyEntries(string commandsAsString)
        {
            using (var reader = new StringReader(commandsAsString))
            {
                string command;
                while ((command = reader.ReadLine()) != null)
                {
                    Debug.Assert(!string.IsNullOrEmpty(command));
                }
            }
        }
    }
}
