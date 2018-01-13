namespace Approximator
{
    public class FingerDetectorMediator : DetectorMediator
    {
        [Inject]
        public FingerDetectorView View { get; private set; }
        protected override DetectorView GetView()
        {
            return View;
        }
    }
}