using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UIKit.Components.Attributes;
using UIKit.Components.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public class InputFieldView : View, IComponentAction, IComponentActionBinder
    {
        private readonly ComponentActionEvent<Action<string>> _didEndEditingEvent = new ComponentActionEvent<Action<string>>();
        private readonly ComponentActionEvent<Action<string>> _valueDidChangeEvent = new ComponentActionEvent<Action<string>>();
        private readonly ComponentActionEvent<Func<string, int, char, char>> _validateInputEvent = new ComponentActionEvent<Func<string, int, char, char>>();

        private AInputField _inputField = default;

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        [ComponentActionBinder]
        public event Action<string> didEndEditing;
        [ComponentActionBinder]
        public event Action<string> valueDidChange;
        [ComponentActionBinder]
        public event Func<string, int, char, char> validateInput;

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            
            TMP_InputField tmpInputField = GetComponent<TMP_InputField>();
            if (tmpInputField)
            {
                _inputField = new InputFieldTMP(tmpInputField, this);

                return;
            }

            InputField inputField = GetComponent<InputField>();
            if (inputField)
            {
                _inputField = new InputFieldUI(inputField, this);

                return;
            }

            throw new MissingComponentException($"Missing TMP_InputField or UI.InputField component.");
        }

        private void OnDestroy()
        {
            UnbindAll();

            if (_inputField == null) return;

            _inputField.Clear();
        }

        #endregion

        #region IComponentActionMultipleBinder

        void IComponentActionBinder.BindAction(UnityEngine.Object target, MethodInfo info, EventInfo eventInfo)
        {
            if (eventInfo == null) return;

            switch (eventInfo.Name)
            {
                case nameof(valueDidChange):
                    {
                        Action<string> action = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
                        if (!_valueDidChangeEvent.AddEvent(action)) return;
                        valueDidChange += action;
                    }
                    break;

                case nameof(didEndEditing):
                    {
                        Action<string> action = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
                        if (!_didEndEditingEvent.AddEvent(action)) return;
                        didEndEditing += action;
                    }
                    break;

                case nameof(validateInput):
                    {
                        Func<string, int, char, char> func = (Func<string, int, char, char>)info.CreateDelegate(typeof(Func<string, int, char, char>), target);
                        if (!_validateInputEvent.AddEvent(func)) return;
                        validateInput += func;
                    }
                    break;
            }
        }

        void IComponentActionBinder.UnbindActions()
        {
            UnbindAll();
        }

        #endregion

        internal void DidEndEditingAction(string newText) => didEndEditing?.Invoke(newText);
        internal void ValueDidChangeAction(string newText) => valueDidChange?.Invoke(newText);
        internal char ValidateInputAction(string text, int charIndex, char addedChar)
            => validateInput?.Invoke(text, charIndex, addedChar) ?? addedChar;

        private void UnbindAll()
        {
            _valueDidChangeEvent.UnbindAll(each => valueDidChange -= each);
            _didEndEditingEvent.UnbindAll(each => didEndEditing -= each);
            _validateInputEvent.UnbindAll(each => validateInput -= each);
        }
    }
}
