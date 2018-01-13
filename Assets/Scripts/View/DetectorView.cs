using System.Collections;
using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.dispatcher.eventdispatcher.api;

namespace Approximator
{
    public class DetectorView : View
    {
        [Inject]
        public IEventDispatcher Dispatcher { get; private set; }

        internal const string TOUCH = "touch";
        internal const string TOUCH_END = "end";
        internal void Init()
        {
            StartCoroutine(SearchTouch());
        }
        protected virtual bool GetTouch()
        {
            return false;
        }
        protected virtual bool GetTouchUp()
        {
            return false;
        }
        internal virtual Vector3 GetTouchPosition()
        {
            return Vector3.zero;
        }
        IEnumerator SearchTouch()
        {
            while (true)
            {
                if (GetTouch())
                    OnTouch(GetTouchPosition());
                if (GetTouchUp())
                    OnTouchUp();
                yield return null;
            }
        }
        void OnTouch(Vector3 pos)
        {
            Dispatcher.Dispatch(TOUCH);
        }
        void OnTouchUp()
        {
            Dispatcher.Dispatch(TOUCH_END);
        }
    }
}