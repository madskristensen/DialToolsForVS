namespace DialControllerTools
{
    public interface IDialControllerHost
    {
        void RequestActivation(IDialController controller);
        void ReleaseActivation();
    }
}
