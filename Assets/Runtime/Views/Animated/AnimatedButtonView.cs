using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit.Animated
{
    [RequireComponent(typeof(Button))]
    public class AnimatedButtonView : AnimatedView
    {
        private Button _button = default;

        public Action ClickAction { get; set; }
        
        #region Life cycle
        
        protected override void Awake()
        {
            base.Awake();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        #endregion

        private void OnClick() => ClickAction?.Invoke();
    }
}
