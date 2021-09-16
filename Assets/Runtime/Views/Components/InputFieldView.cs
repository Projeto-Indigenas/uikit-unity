using System;
using TMPro;
using UIKit.Components;
using UnityEngine;

namespace UIKit
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldView : View, IComponentAction<string>
    {
        private TMP_InputField _inputField = default;

        public string text
        {
            get => _inputField.text;
            set => _inputField.text = value;
        }

        public Action<string> onEndEditing;

        #region Life cycle

        protected override void Awake()
        {
            base.Awake();
            
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onEndEdit.AddListener(OnEndEdit);
        }

        #endregion

        #region IComponentAction<string>

        Action<string> IComponentAction<string>.action
        {
            get => onEndEditing;
            set => onEndEditing = value;
        }

        #endregion

        private void OnEndEdit(string newValue) => onEndEditing?.Invoke(newValue);
    }
}
