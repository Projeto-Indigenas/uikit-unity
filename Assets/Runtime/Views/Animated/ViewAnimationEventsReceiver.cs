using JetBrains.Annotations;
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

        [UsedImplicitly] public void ViewDidAppear() => _listener.ViewDidAppear();
        [UsedImplicitly] public void ViewDidDisappear() => _listener.ViewDidDisappear();
        
        #endregion
    }
}
