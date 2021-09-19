using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UIKit.Components.Attributes;
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

    public class InputFieldView : View, IComponentAction, IComponentActionBinder
    {
        private AInputField _inputField = default;

        private Action<string> _didEndEditing = default;
        private Action<string> _valueDidChange = default;
        private Func<string, int, char, char> _validateInput = default;

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        [ComponentActionBinder]
        public event Action<string> didEndEditing
        {
            add
            {
                if (_inputField == null) return;
                _inputField.didEndEditing += value;
            }
            remove
            {
                if (_inputField == null) return;
                _inputField.didEndEditing -= value;
            }
        }

        [ComponentActionBinder]
        public event Action<string> valueDidChange
        {
            add
            {
                if (_inputField == null) return;
                _inputField.valueDidChange += value;
            }
            remove
            {
                if (_inputField == null) return;
                _inputField.valueDidChange -= value;
            }
        }

        [ComponentActionBinder]
        public event Func<string, int, char, char> validateInput
        {
            add
            {
                if (_inputField == null) return;
                _inputField.validateInput += value;
            }
            remove
            {
                if (_inputField == null) return;
                _inputField.validateInput -= value;
            }
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
            _inputField.validateInput -= _validateInput;
        }

        #endregion

        #region IComponentActionMultipleBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (eventInfo == null) return;

            switch (eventInfo.Name)
            {
                case nameof(valueDidChange):
                    valueDidChange -= _valueDidChange;

                    _valueDidChange = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
                    break;

                case nameof(didEndEditing):
                    didEndEditing -= _didEndEditing;

                    _didEndEditing = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
                    break;

                case nameof(validateInput):
                    validateInput -= _validateInput;

                    _validateInput = (Func<string, int, char, char>)info.CreateDelegate(typeof(Func<string, int, char, char>), target);
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

            _inputField.validateInput -= _validateInput;
            _inputField.validateInput += _validateInput;
        }
    }
}
