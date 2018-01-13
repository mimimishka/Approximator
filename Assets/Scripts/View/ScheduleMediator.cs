using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using UniRx;

namespace Approximator
{
    public class ScheduleMediator : Mediator
    {
        [Inject]
        public ScheduleView View { get; private set; }
        [Inject]
        public ScheduleConfig Config { get; private set; }
        [Inject]
        public TouchSignal TouchSignal { get; private set; }
        [Inject]
        public OnScheduleTouchSignal ScheduleTouch { get; private set; }
        [Inject]
        public MainModel Model { get; private set; }

        public override void OnRegister()
        {
            Model.Table.ObserveAdd().Subscribe(_ => View.UpdatePoints(Model.Table));
            Model.OLS.Subscribe(f => View.UpdateOLSSchedule(f));
            Model.Lagrange.Subscribe(f => View.UpdateLagrangeSchedule(f));
            TouchSignal.AddListener(View.OnTouch);
            View.Dispatcher.UpdateListener(true, ScheduleView.TOUCH, OnScheduleTouch);
            View.Init(Config);
        }
        void OnScheduleTouch(IEvent evt)
        {
            ScheduleView.Point p = evt.data as ScheduleView.Point;
            ScheduleTouch.Dispatch((int)p.X, p.Y);
        }
    }
}