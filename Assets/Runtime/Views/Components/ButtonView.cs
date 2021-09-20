using System;
using System.Collections.Generic;
using System.Reflection;
using UIKit.Components;
using UIKit.Components.Attributes;
using UIKit.Components.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Button))]
    public class ButtonView : View, IComponentAction, IComponentActionBinder
    {
        private readonly ComponentActionEvent<Action> _buttonPressedEvent = new ComponentActionEvent<Action>();

        private Button _button = default;

        [ComponentActionBinder]
        public event Action buttonPressed;

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();

            _button = GetComponent<Button>();
            _button.onClick.AddListener(ButtonPressed);
        }

        private void OnDestroy()
        {
            UnbindAll();

            if (!_button) return;

            _button.onClick.RemoveAllListeners();
        }

        #endregion

        #region IComponentActionBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (eventInfo == null || !eventInfo.Name.Equals(nameof(buttonPressed))) return;

            Action action = (Action)info.CreateDelegate(typeof(Action), target);
            if (!_buttonPressedEvent.AddEvent(action)) return;
            buttonPressed += action;
        }

        void IComponentActionBinder.UnbindActions()
        {
            UnbindAll();
        }

        #endregion

        private void ButtonPressed() => buttonPressed?.Invoke();

        private void UnbindAll()
        {
            _buttonPressedEvent.UnbindAll(each => buttonPressed -= each);
        }
    }
}
