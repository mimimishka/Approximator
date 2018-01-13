using UnityEngine;

namespace Approximator
{
    public class MouseDetectorView : DetectorView
    {
        protected override bool GetTouch()
        {
            return Input.GetMouseButton(0);
        }
        protected override bool GetTouchUp()
        {
            return Input.GetMouseButtonUp(0);
        }
        internal override Vector3 GetTouchPosition()
        {
            return Input.mousePosition;
        }
    }
}