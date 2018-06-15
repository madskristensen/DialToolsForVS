using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DialToolsForVS.Helpers
{
    public class VsCommands
    {
        public static void Initialize()
        {
            CommandsAsString = ReadCommandsAsString();
            CheckEmptyEntries(CommandsAsString);
            commands = CommandsAsString
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToImmutableArray();
        }

        public static string CommandsAsString { get; private set; }

        private static ImmutableArray<string> commands = new ImmutableArray<string>();

        public static ImmutableArray<string> Commands => commands == ImmutableArray<string>.Empty
            ? (commands = CommandsAsString
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .ToImmutableArray())
            : commands;

        private static string ReadCommandsAsString()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DialToolsForVS.Resources.commands.txt";
            var stream = assembly.GetManifestResourceStream(resourceName);
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [Conditional("Debug")]
        private static void CheckEmptyEntries(string commandsAsString)
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
