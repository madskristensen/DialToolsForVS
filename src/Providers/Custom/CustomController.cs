namespace DialToolsForVS
{
    internal class CustomController : BaseController
    {
        public CustomController() { }
     
        public override string Moniker => CustomControllerProvider.Moniker;
        public override bool CanHandleClick => true;
        public override bool CanHandleRotate => true;

        public override bool OnClick()
        {

            return true;
        }

        public override bool OnRotate(RotationDirection direction)
        {
            return true;
        }
        
    }
}
