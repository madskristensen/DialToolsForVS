namespace DialToolsForVS
{
    [DialControllerProvider(order: 8)]
    internal class FindControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Find.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        public FindControllerProvider() { }
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Find\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new FindController();
        }
    }
}
