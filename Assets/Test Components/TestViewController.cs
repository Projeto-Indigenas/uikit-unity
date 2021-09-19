using UIKit;
using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEngine;

namespace UIKitTests
{
    class TestViewController : ViewController
    {
        [SerializeField] private ComponentBinding _unkownViewTypeBinding = default;
        [SerializeField] private ComponentBinding<InputFieldView> _inputFieldViewBinding = default;
        [SerializeField] private ComponentBinding<ButtonView> _buttonViewBinding = default;
        [SerializeField] private ComponentBinding<DropdownView> _dropdownViewBinding = default;

        [ComponentAction]
        private void ButtonViewAction()
        {
            Debug.Log("This is automatically assigned button action");
        }

        [ComponentAction]
        private void OtherButtonViewAction()
        {
            Debug.Log("This is the other automatically assigned button action");
        }

        [ComponentAction]
        private void DropdownViewAction(DropdownOption selectedOption)
        {
            Debug.Log($"This is automatically assigned dropdown action: {selectedOption.text}");
        }

        [ComponentAction]
        private void OtherDropdownViewAction(DropdownOption selectedOption)
        {
            Debug.Log($"This is the other automatically assigned dropdown action: {selectedOption.text}");
        }

        [ComponentAction]
        private void InputFieldViewAction(string newText)
        {
            Debug.Log($"This is automatically assigned input field action value: {newText}");
        }

        [ComponentAction]
        private void OtherInputFieldViewAction(string newText)
        {
            Debug.Log($"This is the other automatically assigned input field action value: {newText}");
        }

        [ComponentAction]
        private void ValudDidChangeAction(string newText)
        {
            Debug.Log($"Value did change to: {newText}");
        }

        [ComponentAction]
        private char ValidateInput(string text, int index, char newChar)
        {
            Debug.Log($"This is the ValidateInput autoassigned event: text=({text}), index={index}, newChar={newChar}");

            return newChar;
        }
    }
}
