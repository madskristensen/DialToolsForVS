namespace DialToolsForVS
{
    [DialControllerProvider(order: 5)]
    internal class DebugControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Debug.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Debug\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new DebugController(host);
        }
    }
}
