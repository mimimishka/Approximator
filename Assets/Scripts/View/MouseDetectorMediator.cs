namespace Approximator
{
    public class MouseDetectorMediator : DetectorMediator
    {
        [Inject]
        public MouseDetectorView View { get; private set; }
        protected override DetectorView GetView()
        {
            return View;
        }
    }
}