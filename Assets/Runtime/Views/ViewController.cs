using System;
using UIKit.Animated;
using UIKit.Extensions;
using UnityEngine;

namespace UIKit
{
    public interface IViewController
    {
        void Configure(NavigationController navigationController);
        void ViewWillAppearCall(bool animated);
        void ViewDidAppearCall(bool animated);
        void ViewWillDisappearCall(bool animated);
        void ViewDidDisappearCall(bool animated);
    }

    [DefaultExecutionOrder(1)]
    public class ViewController : MonoBehaviour, IViewController
    {
        [SerializeField] private bool _visibleWhenFirstLoaded = default;

        private IAnimatedView _animatedViewInterface = default;

        [field: NonSerialized] protected AAnimatedView view { get; private set; } = default;
        
        [field: NonSerialized] public NavigationController navigationController { get; private set; }

        public bool isInPresentationTransition { get; private set; }
        public bool isBeingPresented { get; private set; }

        public void Present(bool animated = true) => _animatedViewInterface.Show(animated);
        public void Dismiss(bool animated = true) => _animatedViewInterface.Hide(animated);

        #region ViewController's Life cycle
        
        protected virtual void ViewDidLoad()
        {
            //
        }
        
        protected virtual void ViewWillAppear(bool animated)
        {
            isInPresentationTransition = animated;
            isBeingPresented = !animated;
        }

        protected virtual void ViewDidAppear(bool animated)
        {
            isInPresentationTransition = false;
            isBeingPresented = true;
        }

        protected virtual void ViewWillDisappear(bool animated)
        {
            isInPresentationTransition = animated;
            isBeingPresented = animated;
        }

        protected virtual void ViewDidDisappear(bool animated)
        {
            isInPresentationTransition = false;
            isBeingPresented = false;
        }

        #endregion

        #region Life cycle

        protected virtual void Awake()
        {
            Transform thisTransform = transform;
            if (thisTransform.childCount == 0)
            {
                Debug.LogError($"{this} MUST have 1 child view GameObject. ViewController's transform path: {gameObject.GetFullPath()}");
                return;
            }
            view = thisTransform.GetChild(0).GetComponent<AAnimatedView>();
            _animatedViewInterface = view;
            _animatedViewInterface.SetViewController(this);
            
            ViewDidLoad();

            if (_visibleWhenFirstLoaded) return;

            view.Hide(false);
        }

        #endregion

        void IViewController.Configure(NavigationController newNavController)
        {
            if (navigationController != null) return;
            navigationController = newNavController;
        }
        void IViewController.ViewWillAppearCall(bool animated) => ViewWillAppear(animated);
        void IViewController.ViewDidAppearCall(bool animated) => ViewDidAppear(animated);
        void IViewController.ViewWillDisappearCall(bool animated) => ViewWillDisappear(animated);
        void IViewController.ViewDidDisappearCall(bool animated) => ViewDidDisappear(animated);
    }
    
    public class ViewController<TView> : ViewController where TView : AAnimatedView
    {
        [field: NonSerialized] public new TView view { get; set; } = default;

        protected override void ViewDidLoad()
        {
            view = base.view as TView;

            base.ViewDidLoad();
        }
    }
}
