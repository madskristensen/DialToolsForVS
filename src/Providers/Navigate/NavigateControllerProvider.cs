namespace DialToolsForVS
{
    [DialControllerProvider(order: 3)]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Navigation";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Navigate\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new NavigateController();
        }
    }
}
