using System;
using System.Reflection;
using UIKit.Components;
using UIKit.Components.Attributes;
using UIKit.Components.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Slider))]
    public class SliderView : View, IComponentAction, IComponentActionBinder
    {
        private readonly ComponentActionEvent<Action<float>> _valueDidChangeEvent = new ComponentActionEvent<Action<float>>();

        private Slider _slider = default;

        public float value
        {
            get => _slider.value;
            set => _slider.value = value;
        }

        [ComponentActionBinder]
        public event Action<float> valueDidChange;

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();

            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(ValueDidChange);
        }

        private void OnDestroy()
        {
            UnbindAll();

            if (!_slider) return;

            _slider.onValueChanged.RemoveAllListeners();
        }

        #endregion

        #region IComponentActionBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (eventInfo == null || !eventInfo.Name.Equals(nameof(valueDidChange))) return;

            Action<float> action = (Action<float>)info.CreateDelegate(typeof(Action<float>), target);
            if (!_valueDidChangeEvent.AddEvent(action)) return;
            valueDidChange += action;
        }

        void IComponentActionBinder.UnbindActions()
        {
            UnbindAll();
        }

        #endregion

        private void ValueDidChange(float newValue) => valueDidChange?.Invoke(newValue);

        private void UnbindAll()
        {
            _valueDidChangeEvent.UnbindAll(each => valueDidChange -= each);
        }
    }
}
