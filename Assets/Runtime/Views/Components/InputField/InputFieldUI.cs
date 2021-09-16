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

        public override event Action<string> onEndEditing;

        public InputFieldUI(InputField inputField)
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
