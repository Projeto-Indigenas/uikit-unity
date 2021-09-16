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
        private void NoParametersMethod()
        {
            Debug.Log("This is automatically assigned");
        }

        [ComponentAction]
        private void ParameterizedMethod(ADropdownOption selectedOption)
        {

        }

        [ComponentAction]
        private void InputFieldAction(string newText)
        {

        }
    }
}
