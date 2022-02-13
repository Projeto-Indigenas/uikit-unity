using System;
using System.Reflection;
using UIKit.Components;
using UIKit.Components.Attributes;
using UIKit.Components.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit.Views.Components
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleView : View, IComponentAction, IComponentActionBinder
    {
        private readonly ComponentActionEvent<Action<bool>> _valueDidChangeEvent = new ComponentActionEvent<Action<bool>>();

        private Toggle _toggle = default;

        [ComponentActionBinder]
        public event Action<bool> valueDidChange;

        public bool isOn
        {
            get => _toggle.isOn;
            set => _toggle.isOn = value;
        }

        protected override void Awake()
        {
            base.Awake();

            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            UnbindAll();

            if (!_toggle) return;

            _toggle.onValueChanged.RemoveAllListeners();
        }

        private void OnValueChanged(bool value) => valueDidChange?.Invoke(value);

        private void UnbindAll()
        {
            _valueDidChangeEvent.UnbindAll(each => valueDidChange -= each);
        }

        #region IComponentActionBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (eventInfo == null || !eventInfo.Name.Equals(nameof(valueDidChange))) return;

            Action<bool> action = (Action<bool>)info.CreateDelegate(typeof(Action<bool>), target);
            if (!_valueDidChangeEvent.AddEvent(action)) return;
            valueDidChange += action;
        }

        void IComponentActionBinder.UnbindActions()
        {
            UnbindAll();
        }

        #endregion
    }
}
