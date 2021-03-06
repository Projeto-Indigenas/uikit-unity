using System;
using TMPro;
using UIKit.Extensions;

namespace UIKit
{
    internal class DropdownTMP : ADropdown
    {
        private static readonly Func<DropdownOption, TMP_Dropdown.OptionData> _mapper = each =>
        {
            return new TMP_Dropdown.OptionData(each.text, each.image);
        };

        private readonly TMP_Dropdown _dropdown = default;

        public DropdownTMP(TMP_Dropdown dropdown, DropdownView view)
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