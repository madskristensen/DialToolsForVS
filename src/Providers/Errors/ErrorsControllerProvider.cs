namespace DialToolsForVS
{
    [DialControllerProvider(order: 6)]
    internal class ErrorsControllerProvider : IDialControllerProvider
    {
        public static string Moniker = KnownProviders.Errors.ToString();

        public IDialController TryCreateController(IDialControllerHost host)
        public ErrorsControllerProvider() { }
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Errors\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new ErrorsController(host);
        }
    }
}
