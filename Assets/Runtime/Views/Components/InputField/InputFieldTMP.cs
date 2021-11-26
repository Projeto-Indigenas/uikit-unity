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

        public InputFieldTMP(TMP_InputField inputField, InputFieldView view)
        {
            _inputField = inputField;
            _inputField.onEndEdit.AddListener(view.DidEndEditingAction);
            _inputField.onValueChanged.AddListener(view.ValueDidChangeAction);
            _inputField.onValidateInput += view.ValidateInputAction;
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
