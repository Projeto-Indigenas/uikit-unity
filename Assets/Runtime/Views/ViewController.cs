using System;
using JetBrains.Annotations;
using UIKit.Animated;
using UnityEngine;
using UnityEngine.UI;

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

    [RequireComponent(typeof(Canvas), typeof(CanvasScaler))]
    public abstract class ViewController : MonoBehaviour, IViewController
    {
        private IAnimatedView _animatedViewInterface = default;

        [field: NonSerialized] protected AnimatedView view { get; private set; } = default;
        
        [UsedImplicitly]
        [field: NonSerialized] public NavigationController navigationController { get; private set; }

        [UsedImplicitly] public bool isInPresentationTransition { get; private set; }
        [UsedImplicitly] public bool isBeingPresented { get; private set; }

        [UsedImplicitly] public void Present(bool animated = true) => _animatedViewInterface.Show(animated);
        [UsedImplicitly] public void Dismiss(bool animated = true) => _animatedViewInterface.Hide(animated);

        #region ViewController's Life cycle
        
        [UsedImplicitly]
        protected virtual void ViewDidLoad()
        {
            //
        }
        
        [UsedImplicitly]
        protected virtual void ViewWillAppear(bool animated)
        {
            isInPresentationTransition = animated;
            isBeingPresented = !animated;
        }

        [UsedImplicitly]
        protected virtual void ViewDidAppear(bool animated)
        {
            isInPresentationTransition = false;
            isBeingPresented = true;
        }

        [UsedImplicitly]
        protected virtual void ViewWillDisappear(bool animated)
        {
            isInPresentationTransition = animated;
            isBeingPresented = animated;
        }

        [UsedImplicitly]
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
            view = thisTransform.GetChild(0).GetComponent<AnimatedView>();
            _animatedViewInterface = view;
            _animatedViewInterface.SetViewController(this);
            
            ViewDidLoad();
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
    
    public class ViewController<TView> : ViewController where TView : AnimatedView
    {
        [UsedImplicitly]
        [field: NonSerialized] private new TView view { get; set; } = default;

        protected override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            view = base.view as TView;
        }
    }
}
