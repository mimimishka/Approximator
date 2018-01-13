using strange.extensions.signal.impl;
using UnityEngine;

namespace Approximator
{
    public class TouchSignal : Signal<Vector3> { }
    public class OnScheduleTouchSignal : Signal<int, float> { }
    public class OnKeyPointChangedSignal : Signal { }
}