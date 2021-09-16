using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UIKit.Extensions;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DropdownView : View, IComponentAction<DropdownOption>, IComponentActionSetup
    {
        private static readonly Func<DropdownOption, TMP_Dropdown.OptionData> _mapper = each => each;

        [SerializeField] private DropdownOption[] _startOptions = default;
        
        private TMP_Dropdown _dropdown = default;

        public event Action<DropdownOption> onValueChanged;

        public void SetOptions(DropdownOption[] options) => _dropdown.options = options?.Map(_mapper)?.AsList();

        protected override void Awake()
        {
            base.Awake();
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(ValueChangedAction);

            if (_startOptions == null || _startOptions.Length == 0) return;

            SetOptions(_startOptions);
        }

        private void OnDestroy() => _dropdown.onValueChanged.RemoveAllListeners();

        private void ValueChangedAction(int index) => onValueChanged?.Invoke((DropdownOption) _dropdown.options[index]);

        #region IComponentAction<ADropdownOption>

        event Action<DropdownOption> IComponentAction<DropdownOption>.action 
        {
            add => onValueChanged += value;
            remove => onValueChanged -= value;
        }

        #endregion

        #region IComponentActionSetup

        void IComponentActionSetup.SetupAction(UnityEngine.Object target, MethodInfo info)
        {
            Action<DropdownOption> action = (Action<DropdownOption>)info.CreateDelegate(typeof(Action<DropdownOption>), target);
            onValueChanged += action;
        }

        #endregion
    }

    [Serializable]
    public class DropdownOption : TMP_Dropdown.OptionData
    {
        protected DropdownOption(string text = null, Sprite image = null) : base(text, image) { }
    }

    public class DropdownOption<TValue> : DropdownOption
    {
        public readonly TValue value;

        public DropdownOption(TValue value, string text, Sprite image = null) : base(text, image) 
            => this.value = value;
    }
}