using UnityEngine;

namespace UIKit
{
    public interface IView
    {
        void SetViewController(IViewController viewController);
        void Show();
        void Hide();
    }

    [DefaultExecutionOrder(0)]
    [RequireComponent(typeof(CanvasGroup))]
    public class View : MonoBehaviour, IView
    {
        protected IViewController _viewController = default;
        protected CanvasGroup _canvasGroup = default;
        
        #region Life cycle

        protected virtual void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

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

            _canvasGroup.alpha = visible ? 1F : 0F;
            _canvasGroup.blocksRaycasts = visible;
            
            if (visible) _viewController?.ViewDidAppearCall(false);
            else _viewController?.ViewDidDisappearCall(false);
        }
    }
}
