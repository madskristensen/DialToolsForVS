namespace DialToolsForVS
{
    [DialControllerProvider(4)]
    internal class EditorControllerProvider : IDialControllerProvider
    {
        public const string Moniker = "Editor";

        public IDialController TryCreateController(IDialControllerHost host)
        {
            string iconFilePath = VsHelpers.GetFileInVsix("Providers\\Editor\\icon.png");
            host.AddMenuItem(Moniker, iconFilePath);

            return new EditorController();
        }
    }
}
