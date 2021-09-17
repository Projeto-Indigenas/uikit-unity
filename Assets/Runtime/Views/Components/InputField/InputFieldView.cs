using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UIKit
{
    public class InputFieldView : View, IComponentAction<string>, IComponentActionSetup
    {
        private AInputField _inputField = default;

        private Action<string> _lateAction = default;

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
                _inputField.onEndEditing += _lateAction;

                _lateAction = null;

                return;
            }

            InputField inputField = GetComponent<InputField>();
            if (inputField)
            {
                _inputField = new InputFieldUI(inputField);
                _inputField.onEndEditing += _lateAction;

                _lateAction = null;

                return;
            }

            throw new MissingComponentException($"Missing TMP_InputField or UI.InputField component.");
        }

        #endregion

        #region IComponentAction<string>

        event Action<string> IComponentAction<string>.action
        {
            add => _inputField.onEndEditing += value;
            remove => _inputField.onEndEditing -= value;
        }

        #endregion

        #region IComponentActionSetup

        void IComponentActionSetup.SetupAction(UnityEngine.Object target, MethodInfo info)
        {
            Action<string> action = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
            
            if (_inputField != null)
            {
                _inputField.onEndEditing += action;

                return;
            }

            _lateAction = action;
        }

        #endregion
    }
}
