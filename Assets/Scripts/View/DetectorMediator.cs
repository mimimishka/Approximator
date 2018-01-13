using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace Approximator
{
    public class DetectorMediator : Mediator
    {
        [Inject]
        public TouchSignal Signal { get; private set; }

        DetectorView view;

        public override void OnRegister()
        {
            view = GetView();
            view.Dispatcher.UpdateListener(true, DetectorView.TOUCH, OnTouch);
            view.Init();
        }
        protected virtual DetectorView GetView()
        {
            return null;
        }
        void OnTouch(IEvent evt)
        {
            Signal.Dispatch(view.GetTouchPosition());
        }
    }
}