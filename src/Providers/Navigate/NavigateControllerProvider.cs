namespace DialToolsForVS
{
    [DialControllerProvider(order: 3)]
    internal class NavigateControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Navigation.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Navigate\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new NavigateController();
        }
    }
}
