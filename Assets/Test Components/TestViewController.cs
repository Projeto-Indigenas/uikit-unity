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
        private void DropdownViewAction(DropdownOption selectedOption)
        {
            Debug.Log($"This is automatically assigned dropdown action: {selectedOption.text}");
        }

        [ComponentAction]
        private void InputFieldViewAction(string newText)
        {
            Debug.Log($"This is automatically assigned input field action value: {newText}");
        }
    }
}
