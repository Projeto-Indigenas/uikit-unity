using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UIKit.Extensions;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class DropdownView : View
    {
        private static readonly Func<DropdownOption, TMP_Dropdown.OptionData> _mapper = each => each;
        
        private TMP_Dropdown _dropdown = default;

        [UsedImplicitly] public Action<DropdownOption> onValueChanged; 

        [UsedImplicitly]
        public void SetOptions(DropdownOption[] options) => _dropdown.options = options.Map(_mapper).AsList();

        protected override void Awake()
        {
            base.Awake();
            _dropdown = GetComponent<TMP_Dropdown>();
            _dropdown.onValueChanged.AddListener(ValueChangedAction);
        }

        private void OnDestroy() => _dropdown.onValueChanged.RemoveAllListeners();

        private void ValueChangedAction(int index) => onValueChanged.Invoke((DropdownOption) _dropdown.options[index]);
    }

    public abstract class DropdownOption : TMP_Dropdown.OptionData
    {
        protected DropdownOption(string text = null, Sprite image = null) : base(text, image) { }
    }

    public class DropdownOption<TValue> : DropdownOption
    {
        [UsedImplicitly] public readonly TValue value;

        public DropdownOption(TValue value, string text, Sprite image = null) : base(text, image) 
            => this.value = value;
    }
}