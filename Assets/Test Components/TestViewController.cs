using System.ComponentModel;
using UIKit;
using UIKit.Components;
using UIKit.Components.Attributes;
using UnityEngine;

namespace UIKitTests
{
    class TestViewController : ViewController
    {
        [SerializeField] private ComponentBinding _unkownViewBinding = default;
        [SerializeField] private ComponentBinding<InputFieldView> _inputFieldViewBinding = default;
        [SerializeField] private ComponentBinding<ButtonView> _buttonViewBinding = default;
        [SerializeField] private ComponentBinding<DropdownView> _dropdownViewBinding = default;
        [SerializeField] private ComponentBinding<SliderView> _sliderViewBinding = default;

        private View _view => _unkownViewBinding;
        private InputFieldView _inputFieldView => _inputFieldViewBinding;
        private ButtonView _buttonView => _buttonViewBinding;
        private DropdownView _dropdownView => _dropdownViewBinding;
        private SliderView _sliderView => _sliderViewBinding;

        [ComponentAction]
        private void ButtonViewAction()
        {
            Debug.Log("This is automatically assigned button action");

            _inputFieldView.text = "Button pressed!";
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

        [ComponentAction]
        private void ValueDidChangeSlider(float value)
        {
            Debug.Log($"ValueDidChangeSlider: {value}");
        }
    }
}
