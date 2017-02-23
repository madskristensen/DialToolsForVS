namespace DialToolsForVS
{
    public interface IDialControllerProvider
    {
        IDialController TryCreateController();
    }
}
