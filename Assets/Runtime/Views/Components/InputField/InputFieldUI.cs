using System;
using UnityEngine.UI;

namespace UIKit
{
    internal class InputFieldUI : AInputField
    {
        private readonly InputField _inputField;

        public override string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        public InputFieldUI(InputField inputField)
        {
            _inputField = inputField;
            _inputField.onEndEdit.AddListener(OnEndEditing);
        }

        public override void Clear()
        {
            if (!_inputField) return;

            _inputField.onEndEdit.RemoveAllListeners();
        }
    }
}
