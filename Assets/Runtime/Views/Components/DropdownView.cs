using System;
using TMPro;
using UIKit.Components;
using UIKit.Extensions;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DropdownView : AView, IComponentAction<ADropdownOption>
    {
        private static readonly Func<ADropdownOption, TMP_Dropdown.OptionData> _mapper = each => each;
        
        private TMP_Dropdown _dropdown = default;

        public Action<ADropdownOption> action { get; set; }

        public void SetOptions(ADropdownOption[] options) => _dropdown.options = options.Map(_mapper).AsList();

        protected override void Awake()
        {
            base.Awake();
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(ValueChangedAction);
        }

        private void OnDestroy() => _dropdown.onValueChanged.RemoveAllListeners();

        private void ValueChangedAction(int index) => action.Invoke((ADropdownOption) _dropdown.options[index]);
    }

    public abstract class ADropdownOption : TMP_Dropdown.OptionData
    {
        protected ADropdownOption(string text = null, Sprite image = null) : base(text, image) { }
    }

    public class DropdownOption<TValue> : ADropdownOption
    {
        public readonly TValue value;

        public DropdownOption(TValue value, string text, Sprite image = null) : base(text, image) 
            => this.value = value;
    }
}