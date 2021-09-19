using System;
using System.Reflection;
using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Slider))]
    public class SliderView : View, IComponentAction, IComponentActionBinder
    {
        private Slider _slider = default;
        private Action<float> _valueDidChange = default;

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

            BindAction();
        }

        private void OnDestroy()
        {
            if (!_slider) return;

            _slider.onValueChanged.RemoveAllListeners();
            valueDidChange -= _valueDidChange;
        }

        #endregion

        #region IComponentActionBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (_valueDidChange != null) valueDidChange -= _valueDidChange;

            if (eventInfo == null || !eventInfo.Name.Equals(nameof(valueDidChange))) return;

            _valueDidChange = (Action<float>)info.CreateDelegate(typeof(Action<float>), target);

            BindAction();
        }

        #endregion

        private void ValueDidChange(float newValue)
        {
            valueDidChange?.Invoke(newValue);
        }

        private void BindAction()
        {
            if (!_slider) return;

            valueDidChange -= _valueDidChange;
            valueDidChange += _valueDidChange;
        }
    }
}
