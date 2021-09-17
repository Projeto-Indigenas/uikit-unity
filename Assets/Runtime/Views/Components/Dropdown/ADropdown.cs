using System;

namespace UIKit
{
    internal abstract class ADropdown
    {
        private DropdownOption[] _options = default;

        public event Action<DropdownOption> onValueChanged;

        public virtual void SetOptions(DropdownOption[] options)
        {
            _options = options;
        }

        public abstract void Clear();

        protected void ValueChangedAction(int index)
        {
            onValueChanged?.Invoke(_options[index]);
        }
    }
}