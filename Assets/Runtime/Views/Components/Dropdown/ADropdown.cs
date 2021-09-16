using System;

namespace UIKit
{
    internal abstract class ADropdown
    {
        public abstract event Action<DropdownOption> onValueChanged;

        public abstract void SetOptions(DropdownOption[] options);

        public abstract void Clear();
    }
}