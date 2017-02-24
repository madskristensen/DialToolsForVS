namespace DialToolsForVS
{
    [DialControllerProvider(order: 6)]
    internal class ErrorsControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Errors";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Errors\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new ErrorsController(host);
        }
    }
}
