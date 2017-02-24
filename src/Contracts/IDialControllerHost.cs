namespace DialToolsForVS
{
    public interface IDialControllerHost
    {
        void AddMenuItem(string moniker, string iconFilePath);
        void RequestActivation(string moniker);
        void ReleaseActivation();
    }
}
