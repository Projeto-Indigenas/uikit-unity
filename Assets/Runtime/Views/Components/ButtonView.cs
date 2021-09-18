using System;
using System.Reflection;
using UIKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : View, IComponentAction, IComponentActionBinder
    {
        private Button _button = default;
        private Action _buttonPressed = default;

        public event Action buttonPressed;

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();

            _button = GetComponent<Button>();
            _button.onClick.AddListener(ButtonPressed);

            BindAction();
        }

        private void OnDestroy()
        {
            if (!_button) return;

            _button.onClick.RemoveAllListeners();
            buttonPressed -= _buttonPressed;
        }

        #endregion

        #region IComponentActionBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info)
        {
            _buttonPressed = (Action)info.CreateDelegate(typeof(Action), target);
            
            BindAction();
        }

        #endregion

        private void ButtonPressed() => buttonPressed?.Invoke();

        private void BindAction()
        {
            if (!_button) return;

            buttonPressed -= _buttonPressed;
            buttonPressed += _buttonPressed;
        }
    }
}
