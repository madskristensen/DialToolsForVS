namespace DialToolsForVS
{
    [DialControllerProvider(order: 2)]
    internal class ZoomControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Zoom.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        public ZoomControllerProvider() { }
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Zoom\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new ZoomController();
        }
    }
}
