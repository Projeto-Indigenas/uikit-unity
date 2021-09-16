using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    [RequireComponent(typeof(Slider))]
    public class SliderView : View
    {
        private Slider _slider = default;

        public float value
        {
            get => _slider.value;
            set => _slider.value = value;
        }
        
        public Action<float> valueDidChange { get; set; }

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();

            _slider = GetComponent<Slider>();
            _slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy() => _slider.onValueChanged.RemoveAllListeners();

        private void OnValueChanged(float newValue) => valueDidChange?.Invoke(newValue);

        #endregion
    }
}
