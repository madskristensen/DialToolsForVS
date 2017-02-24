namespace DialToolsForVS
{
    [DialControllerProvider(order: 2)]
    internal class ZoomControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Zoom";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Zoom\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new ZoomController();
        }
    }
}
