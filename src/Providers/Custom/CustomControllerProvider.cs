namespace DialToolsForVS
{
    [DialControllerProvider(order: 9)]
    internal class CustomControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Custom.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Custom\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new CustomController();
        }
    }
}
