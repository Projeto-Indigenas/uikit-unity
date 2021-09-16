using System;
using System.Reflection;
using UIKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : View, IComponentAction, IComponentActionSetup
    {
        private Button _button = default;

        public event Action onButtonAction;

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

        event Action IComponentAction.action
        {
            add => onButtonAction += value;
            remove => onButtonAction -= value;
        }

        #endregion

        #region IComponentActionSetup

        void IComponentActionSetup.SetupAction(UnityEngine.Object target, MethodInfo info)
        {
            Action action = (Action)info.CreateDelegate(typeof(Action), target);
            onButtonAction += action;
        }

        #endregion

        private void InvokeAction() => onButtonAction?.Invoke();
    }
}
