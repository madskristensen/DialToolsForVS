using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DialToolsForVS.Helpers
{
    public class VsCommands
    {
        public static void Initialize()
        {
            Commands = SetupCommands();
        }

        public static IEnumerable<string> Commands { get; private set; }

        private static IEnumerable<string> SetupCommands()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "DialToolsForVS.Resources.commands.txt";
            var list = new List<string>();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var command = string.Empty;
                    while ((command = reader.ReadLine()) != null)
                    {
                        yield return command;
                    }
                }
            }
        }
    }
}
