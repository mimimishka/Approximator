using strange.extensions.command.impl;

namespace Approximator
{
    public class DragPointCommand : Command
    {
        [Inject]
        public int X { get; private set; }
        [Inject]
        public float Y { get; private set; }
        [Inject]
        public MainModel Model { get; private set; }
        [Inject]
        public OnKeyPointChangedSignal PointChangedSignal { get; private set; }

        public override void Execute()
        {
            Model.SetPoint(X, Y);
            PointChangedSignal.Dispatch();
        }
    }
}