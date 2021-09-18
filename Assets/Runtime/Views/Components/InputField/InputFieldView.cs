using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    internal enum EInputFieldActionType
    {
        didEndEditing,
        valueDidChange,
        textDidChange,
    }

    public class InputFieldView : View, IComponentAction<string>, IComponentActionBinder<EInputFieldActionType>
    {
        private AInputField _inputField = default;

        private Action<string> _didEndEditing = default;
        private Action<string> _valueDidChange = default;
        private Func<string, int, char, char> _textDidChange = default;

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            
            TMP_InputField tmpInputField = GetComponent<TMP_InputField>();
            if (tmpInputField)
            {
                _inputField = new InputFieldTMP(tmpInputField);
                
                BindAction();

                return;
            }

            InputField inputField = GetComponent<InputField>();
            if (inputField)
            {
                _inputField = new InputFieldUI(inputField);
                
                BindAction();

                return;
            }

            throw new MissingComponentException($"Missing TMP_InputField or UI.InputField component.");
        }

        private void OnDestroy()
        {
            if (_inputField == null) return;

            _inputField.Clear();
            _inputField.didEndEditing -= _didEndEditing;
            _inputField.valueDidChange -= _valueDidChange;
            _inputField.validateInput -= _textDidChange;
        }

        #endregion

        #region IComponentActionMultipleBinder

        void IGenericComponentActionBinder.BindAction(uint actionType, UnityEngine.Object target, MethodInfo info)
        {
            switch ((EInputFieldActionType)actionType)
            {
                case EInputFieldActionType.didEndEditing:
                    _didEndEditing = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
                    break;
                case EInputFieldActionType.valueDidChange:
                    _valueDidChange = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
                    break;
                case EInputFieldActionType.textDidChange:
                    _textDidChange = (Func<string, int, char, char>)info.CreateDelegate(typeof(Func<string, int, char, char>), target);
                    break;
            }

            BindAction();
        }

        #endregion

        private void BindAction()
        {
            if (_inputField == null) return;

            _inputField.didEndEditing -= _didEndEditing;
            _inputField.didEndEditing += _didEndEditing;

            _inputField.valueDidChange -= _valueDidChange;
            _inputField.valueDidChange += _valueDidChange;

            _inputField.validateInput -= _textDidChange;
            _inputField.validateInput += _textDidChange;
        }
    }
}
