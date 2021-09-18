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

        public InputFieldTMP(TMP_InputField inputField)
        {
            _inputField = inputField;
            _inputField.onEndEdit.AddListener(DidEndEditing);
            _inputField.onValueChanged.AddListener(ValueDidChange);
            _inputField.onValidateInput = ValidateInput;
        }

        public override void Clear()
        {
            if (!_inputField) return;

            _inputField.onEndEdit.RemoveAllListeners();
            _inputField.onValueChanged.RemoveAllListeners();
            _inputField.onValidateInput = null;
        }
    }
}
