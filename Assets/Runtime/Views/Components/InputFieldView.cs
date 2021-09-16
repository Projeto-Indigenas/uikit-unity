using System;
using System.Reflection;
using TMPro;
using UIKit.Components;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldView : View, IComponentAction<string>, IComponentActionSetup
    {
        private TMP_InputField _inputField = default;

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        public event Action<string> onEndEditing;

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onEndEdit.AddListener(OnEndEdit);
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

        private void OnEndEdit(string newValue) => onEndEditing?.Invoke(newValue);
    }
}
