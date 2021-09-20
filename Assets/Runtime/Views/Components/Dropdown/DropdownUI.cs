using System;
using UnityEngine.UI;
using UIKit.Extensions;

namespace UIKit
{
    internal class DropdownUI : ADropdown
    {
        private static readonly Func<DropdownOption, Dropdown.OptionData> _mapper = each =>
        {
            return new Dropdown.OptionData(each.text, each.image);
        };

        private readonly Dropdown _dropdown = default;

        public DropdownUI(Dropdown dropdown, DropdownView view)
        {
            _dropdown = dropdown;
            _dropdown.onValueChanged.AddListener(view.ValueDidChangeAction);
        }

        public override void SetOptions(DropdownOption[] options)
        {
            _dropdown.options = options?.Map(_mapper)?.AsList();
        }

        public override void Clear()
        {
            _dropdown.onValueChanged.RemoveAllListeners();
        }
    }
}