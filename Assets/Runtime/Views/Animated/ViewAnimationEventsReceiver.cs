using UnityEngine;

namespace UIKit.Animated
{
    public interface IViewAnimationEventsReceiverListener
    {
        void ViewDidAppear();
        void ViewDidDisappear();
    }
    
    public class ViewAnimationEventsReceiver : MonoBehaviour
    {
        private IViewAnimationEventsReceiverListener _listener = default;
        
        private void Awake() => _listener = GetComponent<IViewAnimationEventsReceiverListener>();

        #region Visibility events

        public void ViewDidAppear() => _listener.ViewDidAppear();
        public void ViewDidDisappear() => _listener.ViewDidDisappear();
        
        #endregion
    }
}
