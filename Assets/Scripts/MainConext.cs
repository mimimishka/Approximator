using UnityEngine;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.command.api;
using strange.extensions.command.impl;


namespace Approximator
{
    public class MainConext : MVCSContext
    {
        public MainConext(MonoBehaviour view) : base(view) { }
        public MainConext(MonoBehaviour view, ContextStartupFlags flags) : base (view, flags) { }
        protected override void addCoreComponents()
        {
            base.addCoreComponents();
            injectionBinder.Unbind<ICommandBinder>();
            injectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>();
        }
        protected override void mapBindings()
        {
            base.mapBindings();
            InjectionBindings();
            MediationBindings();
            ResourcesBindings();
            SignalBindings();
            CommandBindings();
        }
        void SignalBindings()
        {
            injectionBinder.Bind<TouchSignal>().ToSingleton();
            injectionBinder.Bind<OnScheduleTouchSignal>().ToSingleton();
            injectionBinder.Bind<OnKeyPointChangedSignal>();
        }
        void InjectionBindings()
        {
            injectionBinder.Bind<MainModel>().ToSingleton();
            injectionBinder.Bind<int>().ToValue(3).ToName(OLS.Degree);
        }
        void MediationBindings()
        {
            mediationBinder.Bind<ScheduleView>().To<ScheduleMediator>();
            mediationBinder.Bind<MouseDetectorView>().To<MouseDetectorMediator>();
            mediationBinder.Bind<FingerDetectorView>().To<FingerDetectorMediator>();
        }
        void ResourcesBindings()
        {
            injectionBinder.Bind<ScheduleConfig>().ToValue
            (
                Resources.Load("ScheduleConfig") as ScheduleConfig
            );
        }
        void CommandBindings()
        {
            commandBinder.Bind<OnScheduleTouchSignal>().To<DragPointCommand>();
            commandBinder.Bind<OnKeyPointChangedSignal>()
                .InParallel()
                .To<CalculateOLSApproximation>()
                .To<CalculateLagrangeApproximation>();
        }
    }
}