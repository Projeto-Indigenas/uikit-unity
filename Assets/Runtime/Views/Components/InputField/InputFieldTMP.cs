using System;
using TMPro;

namespace UIKit
{
    internal class InputFieldTMP : AInputField
    {
        private readonly TMP_InputField _inputField = default;

        public override string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        public override event Action<string> onEndEditing;
        
        public InputFieldTMP(TMP_InputField inputField)
        {
            _inputField = inputField;
            _inputField.onEndEdit.AddListener(OnEndEditing);
        }

        private void OnEndEditing(string newValue)
        {
            onEndEditing?.Invoke(newValue);
        }
    }
}
