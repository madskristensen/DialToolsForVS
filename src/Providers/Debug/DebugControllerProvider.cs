namespace DialToolsForVS
{
    [DialControllerProvider(order: 5)]
    internal class DebugControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Debug";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Debug\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new DebugController(host);
        }
    }
}
