namespace DialToolsForVS
{
    [DialControllerProvider(order: 1)]
    internal class ScrollControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Scroll";
        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Scroll\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new ScrollController();
        }
    }
}
