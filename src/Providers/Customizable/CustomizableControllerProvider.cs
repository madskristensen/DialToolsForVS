namespace DialToolsForVS
{
    [DialControllerProvider(order: 9)]
    internal class CustomizableControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Customizable.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        public CustomizableControllerProvider() { }
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Customizable\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new CustomizableController();
        }
    }
}
