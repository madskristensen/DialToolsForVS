namespace DialToolsForVS
{
    public interface IDialControllerHost
    {
        void AddMenuItem(string moniker, string iconFilePath);
        void RemoveMenuItem(string moniker);
        void RequestActivation(string moniker);
        void ReleaseActivation();
    }
}
