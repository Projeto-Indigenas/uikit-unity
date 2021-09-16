﻿using UIKit.Animated.Models;
using UnityEngine;

namespace UIKit.Animated
{
    public interface IAnimatedView : IView
    {
        void Show(bool animated);
        void Hide(bool animated);
    }
    
    [RequireComponent(typeof(CanvasGroup), typeof(Animator), typeof(ViewAnimationEventsReceiver))]
    public abstract class AAnimatedView : AView, IAnimatedView, IViewAnimationEventsReceiverListener
    {
        private static readonly Layer _defaultUILayer = new Layer(0, "Default UI Layer");
        private static readonly State _showAnimState = new State("Show", _defaultUILayer);
        private static readonly State _hideAnimState = new State("Hide", _defaultUILayer);

        [SerializeField] private float _animationTransitionDuration = .2F;
        
        private Animator _animator = default;
        private State _animStateToPlay = default;
        
        private void CrossFade()
        {
            if (_animator.IsInTransition(_defaultUILayer)) return;
            if (_animStateToPlay == null) return;
            if (!_animStateToPlay.Is(_animator.GetCurrentAnimatorStateInfo(_defaultUILayer)))
            {
                _animator.CrossFade(_animStateToPlay, _animationTransitionDuration, _defaultUILayer, 0F);
            }
            _animStateToPlay = null;
        }
        
        #region Life cycle
        
        protected override void Awake() 
        {
            base.Awake();
            _animator = GetComponent<Animator>();
            _animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        protected virtual void Update() => CrossFade();

        #endregion

        #region IView

        public void Show(bool animated)
        {
            ViewWillAppearInternal(animated);
            
            if (animated) _animStateToPlay = _showAnimState;
            else ViewDidAppearInternal(false);
        }

        public void Hide(bool animated)
        {
            ViewWillDisappearInternal(animated);
            
            if (animated)
            {
                _animStateToPlay = _hideAnimState;
                return;
            }
            
            ViewDidDisappearInternal(false);
        }

        #endregion

        #region IViewAnimationEventsReceiverListener
        
        void IViewAnimationEventsReceiverListener.ViewDidAppear() => ViewDidAppearInternal(true);
        
        void IViewAnimationEventsReceiverListener.ViewDidDisappear() => ViewDidDisappearInternal(true);

        private void ViewWillAppearInternal(bool animated)
        {
            _viewController?.ViewWillAppearCall(animated);
            _canvasGroup.blocksRaycasts = true;
        }

        private void ViewDidAppearInternal(bool animated)
        {
            if (!animated) _animator.Play(_showAnimState, _defaultUILayer, 1F);
            _viewController?.ViewDidAppearCall(animated);
        }

        private void ViewWillDisappearInternal(bool animated)
        {
            _canvasGroup.blocksRaycasts = false;
            _viewController?.ViewWillDisappearCall(animated);
        }

        private void ViewDidDisappearInternal(bool animated)
        {
            if (!animated) _animator.Play(_hideAnimState, _defaultUILayer, 1F);
            _viewController?.ViewDidDisappearCall(animated);
        }

        #endregion
    }
}
