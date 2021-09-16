using System;
using UIKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : View, IComponentAction
    {
        private Button _button = default;

        public Action onButtonAction { get; private set; } = default;

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(InvokeAction);
        }

        private void OnDestroy() => _button.onClick.RemoveAllListeners();

        #endregion

        #region IComponentAction

        Action IComponentAction.action
        {
            get => onButtonAction;
            set => onButtonAction = value;
        }

        #endregion

        private void InvokeAction() => onButtonAction?.Invoke();
    }
}
