using UnityEngine;

namespace Approximator
{
    public class FingerDetectorView : DetectorView
    {
        bool IsSignleTouch { get { return Input.touchCount == 1; } }
        protected override bool GetTouch()
        {
            if (!IsSignleTouch)
                return false;
            TouchPhase tp = Input.touches[0].phase;
            return tp != TouchPhase.Canceled && tp != TouchPhase.Ended;
        }
        protected override bool GetTouchUp()
        {
            if (!IsSignleTouch)
                return false;
            return Input.touches[0].phase == TouchPhase.Ended;
        }
        internal override Vector3 GetTouchPosition()
        {
            if (!IsSignleTouch)
                return Vector3.zero;
            return Input.touches[0].position;
        }
    }
}