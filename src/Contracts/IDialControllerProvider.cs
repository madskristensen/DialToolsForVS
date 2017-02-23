namespace DialToolsForVS
{
    public interface IDialControllerProvider
    {
        IDialController TryCreateController(IDialControllerHost host);
    }
}
