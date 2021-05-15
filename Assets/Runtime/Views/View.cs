using UnityEngine;

namespace UIKit
{
    public interface IView
    {
        void SetViewController(IViewController viewController);
        void Show();
        void Hide();
    }

    public abstract class View : MonoBehaviour, IView
    {
        protected IViewController _viewController = default;
        protected Canvas _canvas = default;
        
        #region Life cycle

        protected virtual void Awake() => _canvas = GetComponent<Canvas>();

        #endregion

        #region IView

        public void SetViewController(IViewController viewController) => _viewController = viewController;

        void IView.Show() => ToggleVisibility(true);

        void IView.Hide() => ToggleVisibility(false);

        #endregion

        private void ToggleVisibility(bool visible)
        {
            if (visible) _viewController?.ViewWillAppearCall(false);
            else _viewController?.ViewWillDisappearCall(false);
            
            _canvas.enabled = visible;
            
            if (visible) _viewController?.ViewDidAppearCall(false);
            else _viewController?.ViewDidDisappearCall(false);
        }
    }
}
