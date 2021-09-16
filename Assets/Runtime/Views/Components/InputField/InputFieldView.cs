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

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        public event Action<string> onEndEditing
        {
            add => _inputField.onEndEditing += value;
            remove => _inputField.onEndEditing -= value;
        }

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            
            TMP_InputField tmpInputField = GetComponent<TMP_InputField>();
            if (tmpInputField)
            {
                _inputField = new InputFieldTMP(tmpInputField);

                return;
            }

            InputField inputField = GetComponent<InputField>();
            if (inputField)
            {
                _inputField = new InputFieldUI(inputField);

                return;
            }

            throw new MissingComponentException($"Missing TMP_InputField or UI.InputField component.");
        }

        #endregion

        #region IComponentAction<string>

        event Action<string> IComponentAction<string>.action
        {
            add => onEndEditing += value;
            remove => onEndEditing -= value;
        }

        #endregion

        #region IComponentActionSetup

        void IComponentActionSetup.SetupAction(UnityEngine.Object target, MethodInfo info)
        {
            Action<string> action = (Action<string>)info.CreateDelegate(typeof(Action<string>), target);
            onEndEditing += action;
        }

        #endregion
    }
}
